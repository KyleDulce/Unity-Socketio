# Unity-Socket.io Wiki
**Note: This project is not actively maintained**

This project is for [Socket.IO](https://socket.io/) Support in the [Unity Game Engine](https://unity.com/) in a WebGl Project. This project also includes tools to allow testing within the editor. Other Socket.Io libraries do not support WebGl like this library.

This library also allows you to build a multi platform application between Webgl and other platforms using the same code. 

## Table of Contents

* [Getting Started](#Getting-Started)
	* [Supported Versions](#Supported-Versions)
	* [Installation](#Installation)
* [Usage](#Usage)
	* [Demo](#Demo)
* [License](#License)
* [Acknowledgements](#Acknowledgements)

## Getting Started
### Supported Versions
#### Unity Engine
This project was developed and tested in the 2019.4.x LTS editor using the Mono back-end and .NET 4.x. Note that 2020.x changes the WebGL Template. See [here](https://github.com/KyleDulce/Unity-Socketio/wiki/Making-SocketIo-Work-in-2020.x) for making it work in 2020.x.

#### Socket.Io
This project was developed and tested for Socket.Io client version [4.x](https://socket.io/docs/v4) but should also work with servers that uses the same protocol [(Protocol Version 5)](https://github.com/socketio/socket.io-protocol).

### Installation
Download the `Unity-SocketIO.unitypackage` from the [releases](https://github.com/KyleDulce/Unity-Socketio/releases/latest) page. Drag the package into your project in the Unity Editor. The project should auto import by the Unity Editor. 

## Usage
The Documentation can be found in the [wiki](https://github.com/KyleDulce/Unity-Socketio/wiki/Documentation).

When you work on the project to create a WebGl App, switch the platform to standalone to be able to test the project in the editor. Only switch to WebGl when you start building the project.

**Important Note:** Include 
`<script src="https://cdn.socket.io/4.0.0/socket.io.js"></script>`
to the head of your HTML document after you build the WebGl app or else it will not work!. No additional actions are needed for a standalone app.
You can also make these changes inside a WebGl Template. See [here](https://docs.unity3d.com/Manual/webgl-templates.html) for information.

### Demo
Demo.cs
```csharp
using UnityEngine;
using KyleDulce.SocketIo;

public class Demo : MonoBehaviour
{
    Socket s;

    void Start()
    {
		//The url must include "ws://" as the protocol
        s = SocketIo.establishSocketConnection("ws://localhost:3000");
        s.connect();
        s.on("testEvent", call);
    }

    void call(string d) {
        Debug.Log("RECEIVED EVENT: " + d);
        s.emit("testEvent", "test");
    }
}
```
The NodeJs Server
```javascript
const express = require("express");
const app = express();
const port = 3000;

const server = require('http').Server(app);

server.listen(port, () => {
  console.log(`Server listening at port ${port}`);
});

const io = require("socket.io")(server, {
	cors: {
        origin: '*'
    }
});

io.on('connection', (socket) => {
	console.log("Got connection!");
	
	socket.on('testEvent', (data) => {
		console.log("Received test Event " + data);
	});
	
	soc = socket;
	socket.emit("testEvent", "Sending");
});
```
Result:
```
//Unity
Received Event: Sending
//Nodejs Server
Received test Event test
```

## License 
Distributed under the MIT License. See [LICENSE](LICENSE) For more Information.

## Acknowledgements
* [socket.io-client-csharp by doghappy](https://github.com/doghappy/socket.io-client-csharp)
