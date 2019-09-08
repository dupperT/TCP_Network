using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Host : MonoBehaviour
{
    Telepathy.Server server = new Telepathy.Server();
    Encoding utf8 = Encoding.UTF8;

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

    private void Update()
    {
        // server
        if (server.Active)
        {
            // show all new messages
            Telepathy.Message msg;
            while (server.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        ClientConnected(msg);
                        break;
                    case Telepathy.EventType.Data:
                        ProcessData(msg);
                        break;
                    case Telepathy.EventType.Disconnected:
                        ClientDisconnected(msg);
                        break;
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        server.Stop();
    }

    void ClientConnected(Telepathy.Message msg)
    {
        //add player.
        AddPlayer(0, -0, msg.connectionId, Resources.Load("Player/Player") as GameObject, PlayerData.PlayerStatus.Connected);
    }

    void ClientDisconnected(Telepathy.Message msg)
    {
        //remove player
        for (int i = 0; i < Players.Count; i++)
        {
            if(Players[i].connectionId == msg.connectionId)
            {
                //delete that player.
                RemovePlayer(Players[i], i);
            } 
        }
    }

    void ProcessData(Telepathy.Message msg)
    {
        string TheMessage = utf8.GetString(msg.data);
        string[] SplitMessage = TheMessage.Split('/');

    }

    void OnGUI()
    {
        // server
        GUI.enabled = !server.Active;
        if (GUI.Button(new Rect(0, 25, 120, 20), "Start Server"))
        {
            if (!server.Active)
            {
                server.Start(1337);
            }
        }

        GUI.enabled = server.Active;
        if (GUI.Button(new Rect(130, 25, 120, 20), "Stop Server"))
        {
            if (server.Active)
            {
                server.Stop();

                //clear list
                Players.Clear();
            }
        }

        GUI.enabled = true;
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
}