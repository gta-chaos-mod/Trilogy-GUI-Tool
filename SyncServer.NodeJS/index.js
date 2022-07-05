/*
	Config
*/
const port = 12312;
const logs = true;
const debugLogs = false;

const WebSocket = require('ws');
const wss = new WebSocket.Server({ port });

const Channel = require('./channel');
const Channels = new Map();

const Users = new Map();

function log(text) {
	if (!logs) return;

	console.log(text);
}

console.log(`Websocket listening on port ${port}.`);
wss.on('connection', ws => {
	ws.on('message', raw => {
		const data = JSON.parse(raw);

		if (debugLogs) {
			console.log(data);
		}

		if (data.Type == 0) {
			data.Channel = data.Channel.toLowerCase();
			data.Username = data.Username.toLowerCase();

			if (Users.has(data.Username)) {
				ws.send(JSON.stringify({ Type: 1 }));

				log(`Username "${data.Username}" already in use.`);

				return;
			}
			ws._username = data.Username;
			Users.set(data.Username, ws);

			log(`${data.Username} connected - Version: ${data.Version}`);

			if (Channels.has(data.Channel) && !!Channels.get(data.Channel)) {
				const c = Channels.get(data.Channel);
				if (!c) return;

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

					log(`${data.Username} joined channel ${c.getRoomName()} as client.`);
				}
			}
			else {
				const c = new Channel(data.Channel, data.Version, data.Username);
				Channels.set(data.Channel, c);
				ws.send(JSON.stringify({ Type: 0, IsHost: true, HostUsername: data.Username }));

				ws._channel = data.Channel;

				log(`${data.Username} joined channel ${c.getRoomName()} as host.`);
			}
		}

		else if (data.Type == 12) { // Chat Message
			const c = Channels.get(ws._channel);

			for (const u of c.getUsers()) {
				Users.get(u).send(JSON.stringify({ Type: 12, Username: data.Username, Message: data.Message }));
			}

			log(`User ${data.Username} in channel ${c.getRoomName()} said: ${data.Message}`);
		}

		else if (data.Type == 20) { // Time Update
			const c = Channels.get(ws._channel);
			if (!c) return;

			if (c.getHost() != ws._username) return;

			for (const u of c.getUsers()) {
				if (u != ws._username) {
					Users.get(u).send(JSON.stringify({ Type: 20, Remaining: data.Remaining, Total: data.Total }));
				}
			}
		}
		else if (data.Type == 21) { // Send Effect
			const c = Channels.get(ws._channel);
			if (!c) return;

			if (c.getHost() != ws._username) return;

			for (const u of c.getUsers()) {
				Users.get(u).send(JSON.stringify({ Type: 21, Word: data.Word, Duration: data.Duration, Voter: data.Voter, Seed: data.Seed }));
			}

			log(`Effect in channel ${c.getRoomName()}: ${data.Word}, ${data.Duration}`);
		}
		else if (data.Type == 22) { // Votes
			const c = Channels.get(ws._channel);
			if (!c) return;

			if (c.getHost() != ws._username) return;

			for (const u of c.getUsers()) {
				Users.get(u).send(JSON.stringify({ Type: 22, Effects: data.Effects, Votes: data.Votes, LastChoice: data.LastChoice }));
			}
		}
	});

	ws.on('close', () => {
		log(`${ws._username} disconnected.`);

		const c = Channels.get(ws._channel);
		if (c) {
			if (c.getHost() == ws._username) {
				for (const u of c.getUsers()) {
					Users.get(u).send(JSON.stringify({ Type: 2 }));
				}

				Channels.delete(ws._channel);

				log(`Channel ${c.getRoomName()} deleted.`);
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
