class Channel {
	constructor(name, version, username) {
		this.name = name;
		this.version = version;
		this.users = new Set([username]);
		this.host = username;
	}

	getRoomName() {
		return this.name;
	}

	getVersion() {
		return this.version;
	}

	getUsers() {
		return this.users;
	}

	getHost() {
		return this.host;
	}

	addUser(username) {
		if (!this.users.has(username)) {
			this.users.add(username);
		}
	}

	hasUser(username) {
		return this.users.has(username);
	}

	removeUser(username) {
		this.users.delete(username);
	}
}

module.exports = Channel;
