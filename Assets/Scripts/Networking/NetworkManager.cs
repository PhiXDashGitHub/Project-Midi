using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public new string name;
    public float timeOut = 5;
    public int lobbyID;
    public string lobbyKey;
    public List<string> players = new List<string>();

    [Header("Scenes")]
    public int noteEditorScene;
    public int ratingScene;

    public void Awake()
    {
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
