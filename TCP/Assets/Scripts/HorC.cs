using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorC : MonoBehaviour
{

    private void Awake()
    {
        Screen.SetResolution(800, 600, false);
    }
    void OnGUI()
    {
        GUI.enabled = true;

        if (GUI.Button(new Rect(0, 0, 120, 20), "Client"))
        {
            transform.gameObject.AddComponent<Client>();
            Destroy(transform.gameObject.GetComponent<HorC>());
        }

        if (GUI.Button(new Rect(130, 0, 120, 20), "Host"))
        {
            transform.gameObject.AddComponent<Host>();
            Destroy(transform.gameObject.GetComponent<HorC>());
        }
    }
}
