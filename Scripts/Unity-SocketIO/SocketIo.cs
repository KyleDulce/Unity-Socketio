using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace KyleDulce.SocketIo {
    public class SocketIo 
    {

        //only needed for webgl
        private static readonly string SOCKET_GAMEOBJECT_NAME = "SocketIo_Ref";

        private static byte _protocol = 0;
        public static byte protocol {
            get {
                if(_protocol == 0) {
                    #if UNITY_WEBGL
                        _protocol = GetProtocol();
                    #else
                        _protocol = 5;
                    #endif
                }
                return _protocol;
            }
        }

        private static Dictionary<int, Socket> EnabledSockets = new Dictionary<int, Socket>();

        public static Socket establishSocketConnection(string Url) => establishSocketConnection(Url, "");

        public static Socket establishSocketConnection(string Url, string options) {

            #if UNITY_WEBGL
                //check for gameobject
                if(GameObject.Find(SOCKET_GAMEOBJECT_NAME) == null) {
                    Debug.Log("Generating SocketIO Object");

                    GameObject SocGObj = new GameObject(SOCKET_GAMEOBJECT_NAME);
                    SocGObj.AddComponent<SocketIoInterface>();
                    
                    GameObject.DontDestroyOnLoad(SocGObj);

                    SetupGameObjectName(SOCKET_GAMEOBJECT_NAME);
                }

                int newSocketId = EstablishSocket(Url, options);

                Socket soc = new Socket(newSocketId);
                EnabledSockets.Add(newSocketId, soc);

                return soc;
            #else
                //generate local id
                int id = -1;
                do{
                    id = (int) Random.Range(1,10000);
                } while(EnabledSockets.ContainsKey(id));
                SocketIOClient.SocketIO client = new SocketIOClient.SocketIO(Url);

                Socket soc = new Socket(id, client);
                EnabledSockets.Add(id ,soc);
                
                return soc;
            #endif
        }

        public static void removeSocket(int id) {
            if(EnabledSockets.TryGetValue(id, out Socket value)) {
                value.disableSocket();
                EnabledSockets.Remove(id);
            } else {
                Debug.LogWarning("Tried to remove a socket but it does not exist, Id: " + id);
            }
        }

        public static bool TryGetSocketById(int id, out Socket soc) {
            return EnabledSockets.TryGetValue(id, out soc);
        }

        public static Socket findSocketWithConnId(string id) {
            foreach(Socket soc in EnabledSockets.Values) {
                if(soc.connectionId == id) {
                    return soc;
                }
            }
            return null;
        }

        //external methods

        #if UNITY_WEBGL
            [DllImport("__Internal")]
            private static extern byte GetProtocol();

            [DllImport("__Internal")]
            private static extern int EstablishSocket(string url, string options);

            [DllImport("__Internal")]
            private static extern string SetupGameObjectName(string name);

            //gameobject for webgl
            public class SocketIoInterface : MonoBehaviour {
                public void callSocketEvent(string data) {
                    SocketEvent ev = JsonUtility.FromJson<SocketEvent>(data);
                    if(EnabledSockets.TryGetValue(ev.SocketId, out Socket soc)) {
                        soc.InvokeEvent(ev.EventName, ev.JsonData);
                    } else {
                        throw new System.NullReferenceException("socket does not exist");
                    }
                }
            }

            private struct SocketEvent {
                public string EventName;
                public int SocketId;
                public string JsonData;
            }
        #endif

    }
}