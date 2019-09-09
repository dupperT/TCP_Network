using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{

    public string ServerIP = "192.168.1.106";
    Encoding utf8 = Encoding.UTF8;

    Telepathy.Client client = new Telepathy.Client();


    [ListDrawerSettings()]
    public List<PlayerData> Players = new List<PlayerData>();

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
        {
            client.Disconnect();
            for (int i = 0; i < Players.Count; i++)
            {
                Destroy(Players[i].PlayerOBJ);
            }
            Players.Clear();
        }

        GUI.enabled = true;
    }

    void OnApplicationQuit()
    {
        client.Disconnect();
    }

    void ProcessData(Telepathy.Message msg)
    {
        string TheMessage = utf8.GetString(msg.data);
        Debug.Log(TheMessage);
        string[] SplitMessage = TheMessage.Split('/');
        
        if(SplitMessage[0].ToLower() == "pj")
        {
            OnPlayerJoin(System.Convert.ToInt32(SplitMessage[1]));
        }

        if (SplitMessage[0].ToLower() == "pl")
        {
            ClientDisconnected(System.Convert.ToInt32(SplitMessage[1]));
        }


    }

    void AddPlayer(float Horizontal, float Vertical, int connectionID, GameObject PlayerOBJ, PlayerData.PlayerStatus playerStatus)
    {
        PlayerData playerData = new PlayerData
        {
            Vertical = Horizontal,
            Horizontal = Vertical,
            connectionId = connectionID,
            PlayerOBJ = Instantiate(PlayerOBJ),
            playerStatus = PlayerData.PlayerStatus.Connected
        };

        Players.Add(playerData);
    }

    void RemovePlayer(PlayerData playerData, int index)
    {
        Destroy(playerData.PlayerOBJ);
        Players.RemoveAt(index);
    }

    void OnPlayerJoin(int connectionToJoin)
    {
        bool exists = false;

        foreach (var player in Players)
        {
            if(player.connectionId == connectionToJoin)
            {
                exists = true;
            }
        }

        if(!exists)
            AddPlayer(0, 0, connectionToJoin, Resources.Load("Player/Player") as GameObject, PlayerData.PlayerStatus.Connected);
    }

    void ClientDisconnected(int connectionID)
    {
        //remove player
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].connectionId == connectionID)
            {
                //delete that player.
                RemovePlayer(Players[i], i);
            }
        }
    }

}