var SocketPlugin = {

    $Data: {
        SocketGameObjectName: "",
        sockets: new Map(),
        //send to unity stuff
        CallUnityEvent: function(id, event, data) {
            var JsonData = null
            if(data != null) {
                JsonData = data
            }
            unityInstance.SendMessage(Data.SocketGameObjectName, 'callSocketEvent', JSON.stringify({
                EventName: event,
                SocketId: id,
                JsonData: JsonData
            }));
        },
    },

    SetupGameObjectName: function(str) {
        Data.SocketGameObjectName = Pointer_stringify(str);
        Data.sockets = new Map();
    },

    GetProtocol: function() {
        if(io != undefined)
            return io.getProtocol;
        else {
            console.error("SocketIO io object not found! Did you forget to include Reference in header?");
            throw new Error("SocketIO object not found! Did you forget to include Reference in header?");
        }
    },

    EstablishSocket: function(url_raw, options_raw) {
        if(io != undefined) {
            const url = Pointer_stringify(url_raw);
            const options = Pointer_stringify(options_raw); //string of user options selected

            var soc;
            if(options.length > 0) 
                soc = io(url, JSON.parse(options));
            else 
                soc = io(url);
            
            var id = 0;
            do {
                //generate an id between 1 and 10000
                id = Math.floor(Math.random() * 10000) + 1;
            } while(Data.sockets.has(id));

            Data.sockets.set(id, soc);

            var cur = this;

            soc.onAny(function(event, args) {
                Data.CallUnityEvent(id, event, args);
            });

            return id;
        } else {
            console.error("SocketIO io object not found! Did you forget to include Reference in header?");
            throw new Error("SocketIO object not found! Did you forget to include Reference in header?");
        }
    },

    //Socket Object stuff

    Socket_IsConnected: function(id) {
        return Data.sockets.get(id).connected;
    },

    Socket_Connect: function(id) {
        Data.sockets.get(id).connect();
    },

    Socket_Disconnect: function(id) {
        Data.sockets.get(id).disconnect();
    },

    // Socket_Send: function(id, data_raw) {
    //     if(data_raw != null)
    //         Data.sockets.get(id).send(JSON.parse(Pointer_stringify(data_raw)));
    //     else 
    //         Data.sockets.get(id).send(null);
    // },

    Socket_Emit: function(id, event_raw, data_raw) {
        if(Pointer_stringify(data_raw).length == 0) {
            Data.sockets.get(id).emit(Pointer_stringify(event_raw), null);
        } else {
            Data.sockets.get(id).emit(Pointer_stringify(event_raw), Pointer_stringify(data_raw));
        }
    },

    Socket_Get_Conn_Id: function(id) {
        var result = Data.sockets.get(id).id;
        if(result != undefined) {
            var buffersize = lengthBytesUTF8(result) + 1;
            var buffer = _malloc(buffersize);
            stringToUTF8(result, buffer, bufferSize);
            return buffer;
        } else {
            return null;
        }
    },
};
autoAddDeps(SocketPlugin, "$Data");
mergeInto(LibraryManager.library, SocketPlugin);