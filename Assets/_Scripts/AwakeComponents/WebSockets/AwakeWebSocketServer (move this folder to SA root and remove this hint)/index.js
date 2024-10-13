const ks = require('node-key-sender');
const app = express();

const crypto = require('crypto');
const express = require('express');
const { createServer } = require('http');
const WebSocket = require('ws');
const port = 80;

const server = createServer(app);
const wss = new WebSocket.Server({ server });

let clients = [];

wss.on('connection', function(ws) {
  console.log("Client joined");

  clients.push(ws);

  ws.on('message', function(data) {
    if (typeof(data) === "string") {
      console.log("string received from client -> '" + data + "'");
      pressKey(data);
    } else {
      console.log("binary received from client -> " + Array.from(data).join(", ") + "");
    }

    notifyClients(data);
  });

  ws.on('close', function() {
    console.log("Client left");

    var index = clients.indexOf(ws);

    if (index !== -1) {
      clients.splice(index, 1);
    }
  });
});

server.listen(port, function() {
  console.log(`Listening on http://localhost:${port}`);
});

function notifyClients (data) {
  for (let i = 0; i < clients.length; i++)
    clients[i].send(data);
}

function pressKey(key) {
  ks.sendKey(key);
}
