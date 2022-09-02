# Chaos Mod Sync Server
This is a tool that allows for syncing Chaos Mod effects and settings across multiple clients for race purposes.

# What is synced between clients
* Effect timer
* Active effects
* Seed
* Rapidfire

# ...and what's not?
* config.toml
* Twitch messages during Chat Voting

# Requirements
* Node.js

# How to install
Run `npm install` in this directory, then run `npm start`.

If npm is throwing dependency errors at you, you may need to update your Node.js installation.

The server will run on port 12312 by default.

This can be changed in the `index.js` file.

If you're running this locally, you might also need to forward the port you'll be using.

# How to connect
In the Chaos Mod UI you have to enter the URL for the websocket in the Sync tab:

![https://i.imgur.com/ahplwUs.png](https://i.imgur.com/ahplwUs.png)

| Field    | Description                                                                                          |
|----------|------------------------------------------------------------------------------------------------------|
| Server   | The address of the server. The formula is `ws://address:port`                                        |
| Username | The username you'll be using on the server. Not related to the Twitch username used in the GUI tool.              |
| Channel  | The name of the channel on the server you'll be joining. Not related to the Twitch channel set in the GUI tool.    |


# Notes
* The channels are created when they are joined. The first client to connect to a channel is automatically set as host.
* During Chat Voting, the votes are pulled only from the Twitch channel set by the host.
* Although the timer is synced across clients, the New Game start is not, so that has to be done at players' discretion.
