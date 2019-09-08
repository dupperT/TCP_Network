using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    public string ServerIP = "192.168.1.106";
    Encoding utf8 = Encoding.UTF8;

    Telepathy.Client client = new Telepathy.Client();

    void Awake()
    {
        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // client
        if (client.Connected)
        {
            //client.Send(utf8.GetBytes("HV " + Hoz.ToString() + " " + Vert.ToString()));

            // show all new messages
            Telepathy.Message msg;
            while (client.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Debug.Log("Connected");
                        break;
                    case Telepathy.EventType.Data:
                        ProcessData(msg);
                        //Debug.Log("Server Sent: " + utf8.GetString(msg.data));
                        break;
                    case Telepathy.EventType.Disconnected:
                        //Destroy(PlayerClient);
                        Debug.Log("Disconnected");
                        break;
                }
            }
        }
    }

    void OnGUI()
    {
        ServerIP = GUI.TextField(new Rect(0, 25, 240, 20), ServerIP, 25);

        // server
        GUI.enabled = !client.Connected;


        if (GUI.Button(new Rect(0, 50, 120, 20), "Join localhost"))
            client.Connect(ServerIP, 1337);

        GUI.enabled = client.Connected;
        if (GUI.Button(new Rect(130, 50, 120, 20), "Disconnect"))
            client.Disconnect();

        GUI.enabled = true;
    }

    void OnApplicationQuit()
    {
        client.Disconnect();
    }

    void ProcessData(Telepathy.Message msg)
    {
        string TheMessage = utf8.GetString(msg.data);
        string[] SplitMessage = TheMessage.Split('/');
    }
}