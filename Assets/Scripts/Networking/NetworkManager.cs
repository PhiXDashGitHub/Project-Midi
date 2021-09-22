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
