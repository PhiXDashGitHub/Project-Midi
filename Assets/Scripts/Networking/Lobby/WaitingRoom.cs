using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RequestAnswer))]
public class WaitingRoom : MonoBehaviour
{
    [Header("UI")]
    public RequestManager requestManager;
    NetworkManager networkManager;
    
    [HideInInspector]
    public int LobbyID, amoutofplayers, GameSceneBuildIndex = 1;
    [HideInInspector]
    public string LobbyKey;
    [HideInInspector]
    public List<string> players = new List<string>();
    [Header("UI")]
    public TextMeshProUGUI PlayerListText;
    public TextMeshProUGUI LobbyKeyText;
    public TextMeshProUGUI DebugText;


    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        GetPlayers();
    }

    public void Update()
    {
        LobbyKeyText.text = "Game Key: " + LobbyKey;
    }

    public void GetPlayers()
    {
        StartCoroutine(UpdatePlayers());
    }

    public IEnumerator UpdatePlayers()
    {
        DebugText.text = players.Count + "/" + amoutofplayers + "Players Joined ...";

        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + LobbyID, this.gameObject);

        float time = 0;
        while (this.GetComponent<RequestAnswer>().Message.Length < 1)
        {
            if (time > networkManager.Timeout)
            {
                break;
            }
            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (this.GetComponent<RequestAnswer>().Message.Length > 1)
        {
            string josn = "{\"Items\":" + this.GetComponent<RequestAnswer>().Message + "}";
            PlayerInfo[] PlayerInfo = JsonHelper.FromJson<PlayerInfo>(josn);
            for (int i = 0; i< PlayerInfo.Length; i++)
            {
                if (!players.Contains(PlayerInfo[i].PlayerName))
                {
                    players.Add(PlayerInfo[i].PlayerName);
                    PlayerListText.text += i+1 + " " + PlayerInfo[i].PlayerName + "\n";
                }
            }

            if (PlayerInfo.Length >= amoutofplayers)
            {
                networkManager.players = players;
                StartGame();
            }
        }
        else
        {
            DebugText.text = "Timeout :(";
        }
        yield return new WaitForSeconds(3);
        StartCoroutine(UpdatePlayers());
    }

    public void StartGame()
    {
        FindObjectOfType<UIManager>().LoadSceneWithAnim(1);
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string PlayerName;

        public static PlayerInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PlayerInfo>(jsonString);
        }
    }
}
