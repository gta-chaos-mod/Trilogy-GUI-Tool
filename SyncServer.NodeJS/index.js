// ---------------------------------------
// Config
const port = 12312;
const logs = true;
const debugLogs = false;
const deleteChannelOnHostLeave = false;
// ---------------------------------------

const readline = require('readline');
const rl = readline.createInterface({
	input: process.stdin,
	output: process.stdout
});

const WebSocket = require('ws');
const wss = new WebSocket.Server({ port });

const Channel = require('./channel');
const Channels = new Map();

const Users = new Map();

function log(text) {
	if (!logs) return;

	console.log(new Date(), text);
}

function disconnect(ws) {
	const c = Channels.get(ws._channel);
	if (c) {
		if (deleteChannelOnHostLeave && c.getHost() == ws._username) {
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

				log(`Channel ${c.getRoomName()} deleted.`);
			}
			else {
				for (const u of c.getUsers()) {
					Users.get(u).send(JSON.stringify({ Type: 11, Username: ws._username }));
				}
			}
		}
	}

	Users.delete(ws._username);
}

log(`Websocket listening on port ${port}.`);
wss.on('connection', ws => {
	ws.isAlive = true;
	ws.on('pong', heartbeat);

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
					const isHost = !deleteChannelOnHostLeave && c.getHost() == data.Username;

					for (const u of c.getUsers()) {
						Users.get(u).send(JSON.stringify({ Type: 10, Username: data.Username }));
					}
					c.addUser(data.Username);
					ws.send(JSON.stringify({ Type: 0, IsHost: isHost, HostUsername: c.getHost() }));

					ws._channel = data.Channel;

					log(`${data.Username} ${isHost ? 're-joined' : 'joined'} channel ${c.getRoomName()} as ${isHost ? 'host' : 'client'}.`);
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

		disconnect(ws);
	});

	ws.on('error', e => {
		log(`${ws._username} errored.`);
		console.error(e);

		disconnect(ws);
	})
});

// Heartbeat
function heartbeat() {
	this.isAlive = true;
}

setInterval(() => {
	for (const ws of wss.clients) {
		if (ws.isAlive === false) return ws.terminate();

		ws.isAlive = false;
		ws.ping();
	}
}, 1000 * 30);

// Input parser
rl.on('SIGINT', () => {
	process.exit();
});

rl.on('line', input => {
	const [command, ...args] = input.split(' ');

	if (command == 'kick') {
		const user = args[0];

		if (!user || user == '') {
			console.error('User can\'t be empty.');
			return;
		}

		if (!Users.has(user)) {
			console.error(`User "${user}" could not be found.`);
			return;
		}

		const u = Users.get(user);
		u.terminate();

		console.log(`User "${user}" was kicked.`);
	}

	if (command == 'delete') {
		const channel = args[0];

		if (!channel || channel == '') {
			console.error('Channel can\'t be empty.');
			return;
		}

		if (!Channels.has(channel)) {
			console.error(`Channel "${channel}" could not be found.`);
			return;
		}

		const c = Channels.get(channel);
		for (const u of c.getUsers()) {
			const user = Users.get(u);
			if (!user) continue;

			user.terminate();
		}

		console.log(`Channel "${channel}" was deleted.`);
	}
});