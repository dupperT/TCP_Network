using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public enum PlayerStatus { Disconnected, Connected }

    public float Horizontal, Vertical;

    public int connectionId;
    public GameObject PlayerOBJ;
    public PlayerStatus playerStatus;
}