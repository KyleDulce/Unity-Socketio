using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine; 

namespace KyleDulce.SocketIo {
    public class Socket 
    {
        private int _id = -1;
        public int id {
            get {
                return _id;
            }
            private set {
                _id = value;
            }
        }
        public string connectionId {
            get {
                #if UNITY_WEBGL
                    return Socket_Get_Conn_Id(id);
                #else
                    return client.Id;
                #endif
            }
        }
        public bool connected {
            get {
                #if UNITY_WEBGL
                    return Socket_IsConnected(id);
                #else
                    return client.Connected;
                #endif
            }
        }
        public bool disconnected {
            get {
                return !connected;
            }
        }
        private bool _disabled = false;
        public bool disabled {
            get {
                return _disabled;
            }
            private set {
                _disabled = value;
            }
        }

        private event Action<string> Action_AnyEvents;
        private Dictionary<string, List<Action<string>>> ActionEvents = new Dictionary<string, List<Action<string>>>();

        #if UNITY_WEBGL
            protected internal Socket(int id) {
                this.id = id;
            }
        #else
            private SocketIOClient.SocketIO client;
            protected internal Socket(int id, SocketIOClient.SocketIO client) {
                this.id = id;
                this.client = client;
                client.OnAny((string eventname,SocketIOClient.SocketIOResponse res) => {
                    InvokeEvent(eventname, res.GetValue<string>());
                }); 
            }
        #endif

        public Socket connect() {
            #if UNITY_WEBGL
                Socket_Connect(id);
            #else
                client.ConnectAsync();   
            #endif
            
            return this;
        }

        public Socket open() {
            return connect();
        }

        public Socket disconnect() {
            #if UNITY_WEBGL
                Socket_Disconnect(id);
            #else
                client.DisconnectAsync();
            #endif
            return this;
        }

        public Socket close() {
            return disconnect();
        }

        // public Socket send(string data) {
        //     #if UNITY_WEBGL
        //         Socket_Send(id, data);
        //     #else
        //     #endif
        //     client.send
        //     return this;
        // }

        public Socket emit(string ev, string data) {
            #if UNITY_WEBGL
                if(data == null)
                    Socket_Emit(id, ev, null);
                else
                    Socket_Emit(id, ev, data);
            #else
                client.EmitAsync(ev, data);
            #endif
            return this;
        }

        public Socket on(string ev, Action<string> callback) {
                if(!ActionEvents.ContainsKey(ev)) {
                    ActionEvents.Add(ev, new List<Action<string>>());
                }
                ActionEvents[ev].Add(callback);
            
            return this;
        }

        public Socket off(string ev, Action<string> callback = null) {
                if(callback != null) {
                    if(ActionEvents.TryGetValue(ev, out List<Action<string>> value)) {
                        value.Remove(callback);
                    }
                } else {
                    ActionEvents = new Dictionary<string, List<Action<string>>>();
                }           
            return this;
        }

        public Socket onAny(Action<string> callback) {
            Action_AnyEvents += callback;
            return this;
        }

        public Socket offAny(Action<string> callback = null) {
            if(callback == null) {
                Action_AnyEvents = null;
            } else {
                Action_AnyEvents -= callback;
            }
            return this;
        }

        public void disableSocket() {
            disconnect();
            disabled = true;
        }

        public void InvokeEvent(string ev, string data) {
            Action_AnyEvents?.Invoke(data);

            //invoke event specific events
            if(ActionEvents.TryGetValue(ev, out List<Action<string>> value)) {
                foreach(Action<string> act in value) {
                    act.Invoke(data);
                }
            }
        }

        #if UNITY_WEBGL
            //external methods
            [DllImport("__Internal")]
            private static extern bool Socket_IsConnected(int id);

            [DllImport("__Internal")]
            private static extern string Socket_Get_Conn_Id(int id);

            [DllImport("__Internal")]
            private static extern void Socket_Connect(int id);
            
            [DllImport("__Internal")]
            private static extern void Socket_Disconnect(int id);
            
            // [DllImport("__Internal")]
            // private static extern void Socket_Send(int id, string data);

            [DllImport("__Internal")]
            private static extern void Socket_Emit(int id, string ev, string data);
        #endif

    }
}