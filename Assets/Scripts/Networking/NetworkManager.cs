using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public string Name;
    public float Timeout;
    public int LobbyID;
    public string LobbyKey;
    public List<string> players = new List<string>();


    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
