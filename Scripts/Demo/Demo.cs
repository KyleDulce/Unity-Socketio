using UnityEngine;
using KyleDulce.SocketIo;

public class Demo : MonoBehaviour
{
    Socket s;
    // Start is called before the first frame update
    void Start()
    {
        s = SocketIo.establishSocketConnection("ws://localhost:3000");
        s.connect();
        s.on("testEvent", call);
    }

    void call(string d) {
        Debug.Log("RECIEVED EVENT: " + d);
        s.emit("testEvent", "test");
    }
}
