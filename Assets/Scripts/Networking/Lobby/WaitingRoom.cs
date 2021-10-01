using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RequestAnswer))]
public class WaitingRoom : MonoBehaviour
{
    [Header("References")]
    public RequestManager requestManager;
    NetworkManager networkManager;
    RequestAnswer requestAnswer;
    UIManager uiManager;

    public int gameSceneBuildIndex = 1;
    [HideInInspector] public int LobbyID, amountOfPlayers;
    [HideInInspector] public string LobbyKey;
    [HideInInspector] public List<string> players = new List<string>();

    [Header("UI")]
    public TextMeshProUGUI PlayerListText;
    public TextMeshProUGUI LobbyKeyText;
    public TextMeshProUGUI DebugText;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        requestAnswer = GetComponent<RequestAnswer>();
        uiManager = FindObjectOfType<UIManager>();

        GetPlayers();
    }

    void Update()
    {
        LobbyKeyText.text = "Game Key: " + LobbyKey;
    }

    public IEnumerator UpdatePlayers()
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + LobbyID, gameObject);

        //Wait For Message
        float time = 0;
        while (requestAnswer.Message.Length < 1)
        {
            if (time > networkManager.timeOut)
            {
                break;
            }

            time += Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }

        //If Message received
        if (requestAnswer.Message.Length > 1)
        {
            string json = "{\"Items\":" + requestAnswer.Message + "}";
            PlayerInfo[] PlayerInfo = JsonHelper.FromJson<PlayerInfo>(json);

            //Add Players to UI List
            for (int i = 0; i< PlayerInfo.Length; i++)
            {
                if (!players.Contains(PlayerInfo[i].PlayerName))
                {
                    players.Add(PlayerInfo[i].PlayerName);
                    PlayerListText.text += i + 1 + " " + PlayerInfo[i].PlayerName + "\n";
                    DebugText.text = players.Count + "/" + amountOfPlayers + "Players Joined ...";
                }
            }

            //If all players joined, start the game
            if (PlayerInfo.Length >= amountOfPlayers)
            {
                networkManager.players = players;
                StartGame();
            }
        }
        else
        {
            DebugText.text = "Timeout :(";
        }

        yield return new WaitForSecondsRealtime(2);
        GetPlayers();
    }

    public void GetPlayers()
    {
        StartCoroutine(UpdatePlayers());
    }

    public void StartGame()
    {
        uiManager.LoadSceneWithAnim(gameSceneBuildIndex);
    }
}
