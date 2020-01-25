/*
	Config
*/
const port = 12312;
const logs = false;

const WebSocket = require('ws');
const wss = new WebSocket.Server({ port });

const Channel = require('./channel');
const Channels = new Map();

const Users = new Map();

console.log(`Websocket listening on port ${port}.`);
wss.on('connection', ws => {
	ws.on('message', raw => {
		const data = JSON.parse(raw);

		if (logs) {
			console.log(data);
		}

		if (data.Type == 0) {
			data.Channel = data.Channel.toLowerCase();
			data.Username = data.Username.toLowerCase();

			if (Users.has(data.Username)) {
				ws.send(JSON.stringify({ Type: 1 }));
				return;
			}
			ws._username = data.Username;
			Users.set(data.Username, ws);

			if (Channels.has(data.Channel)) {
				const c = Channels.get(data.Channel);
				if (c.hasUser(data.Username)) {
					ws.send(JSON.stringify({ Type: 1 }));
				}
				else if (c.getVersion() != data.Version) {
					ws.send(JSON.stringify({ Type: 3, Version: c.getVersion() }));
				}
				else {
					for (const u of c.getUsers()) {
						Users.get(u).send(JSON.stringify({ Type: 10, Username: ws._username }));
					}
					c.addUser(data.Username);
					ws.send(JSON.stringify({ Type: 0, IsHost: false, HostUsername: c.getHost() }));

					ws._channel = data.Channel;
				}
			}
			else {
				const c = new Channel(data.Channel, data.Version, data.Username);
				Channels.set(data.Channel, c);
				ws.send(JSON.stringify({ Type: 0, IsHost: true, HostUsername: data.Username }));

				ws._channel = data.Channel;
			}
		}

		else if (data.Type == 12) { // Chat Message
			const c = Channels.get(ws._channel);
			for (const u of c.getUsers()) {
				Users.get(u).send(JSON.stringify({ Type: 12, Username: data.Username, Message: data.Message }));
			}
		}

		else if (data.Type == 20) { // Time Update
			const c = Channels.get(ws._channel);
			if (c.getHost() != ws._username) return;

			for (const u of c.getUsers()) {
				if (u != ws._username) {
					Users.get(u).send(JSON.stringify({ Type: 20, Remaining: data.Remaining, Total: data.Total }));
				}
			}
		}
		else if (data.Type == 21) { // Send Effect
			const c = Channels.get(ws._channel);
			if (c.getHost() != ws._username) return;

			for (const u of c.getUsers()) {
				Users.get(u).send(JSON.stringify({ Type: 21, Word: data.Word, Duration: data.Duration, Voter: data.Voter, Seed: data.Seed }));
			}
		}
		else if (data.Type == 22) { // Votes
			const c = Channels.get(ws._channel);
			if (c.getHost() != ws._username) return;

			for (const u of c.getUsers()) {
				Users.get(u).send(JSON.stringify({ Type: 22, Effects: data.Effects, Votes: data.Votes, LastChoice: data.LastChoice }));
			}
		}
	});

	ws.on('close', () => {
		const c = Channels.get(ws._channel);
		if (c) {
			if (c.getHost() == ws._username) {
				for (const u of c.getUsers()) {
					Users.get(u).send(JSON.stringify({ Type: 2 }));
				}

				Channels.delete(ws._channel);
			}
			else {
				c.removeUser(ws._username);
				if (c.getUsers().size == 0) {
					Channels.delete(ws._channel);
				}
				else {
					for (const u of c.getUsers()) {
						Users.get(u).send(JSON.stringify({ Type: 11, Username: ws._username }));
					}
				}
			}
		}

		Users.delete(ws._username);
	});
});
