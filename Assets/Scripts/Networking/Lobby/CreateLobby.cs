using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(RequestAnswer))]
public class CreateLobby : MonoBehaviour
{
    [Header("References")]
    public RequestManager requestManager;
    NetworkManager networkManager;

    bool isPublic;
    string LobbyKey;
    int LobbyID;

    string playerName;
    int amountOfPlayers = 4;
    public string[] instruments;

    public int lobbyKeyLength = 5;

    [Header("UI")]
    public TextMeshProUGUI LobbyKeyText;
    public TextMeshProUGUI PrivateText;
    public GameObject WaitingRoom;
    public TMP_InputField PlayerName;
    public Button CreateGame;
    public TextMeshProUGUI ErrorText;

    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        CreateLobbyKey();
        SetLobbyState();

        instruments = new string[InstrumentSelection.maxAmountOfInstruments];

        for (int i = 0; i < instruments.Length; i++)
        {
            instruments[i] = "";
        }

        LoadPlayerName();
    }

    //Saves the Playername
    public void SavePlayerName()
    {
        PlayerPrefs.SetString("PlayerName", PlayerName.text);
    }

    //Loads the saved Playername
    public void LoadPlayerName()
    {
        PlayerName.text = PlayerPrefs.GetString("PlayerName");
    }

    //Checks joining conditions
    public void CheckForOpen()
    {
        if (PlayerName.text.Length < 1)
        {
            ErrorText.text = "Please enter a Name";
            CreateGame.interactable = true;
            return;
        }
        else if (instruments.Length < 1)
        {
            ErrorText.text = "Please select an Instrument";
            CreateGame.interactable = true;
            return;
        }

        StartCoroutine(CheckforOpenLobbys());
    }
    
    //Creates a new Lobbykey
    public void CreateLobbyKey()
    {
        for (int i = 0; i< lobbyKeyLength; i++)
        {
            LobbyKey += Random.Range(0, 9).ToString();
        }

        LobbyKeyText.text = LobbyKey;
    }

    //Sets the Lobbystate (private/open)
    public void SetLobbyState()
    {
        isPublic = !isPublic;

        if (!isPublic)
        {
            PrivateText.text = "Game is private";
        }
        else
        {
            PrivateText.text = "Game is open";
        }

        Debug.Log("Bool: " + isPublic);
    }

    //Sets the amount of Players which can join the lobby
    public void SetAmountOfPlayers(int i) 
    {
        amountOfPlayers = i;
    }

    //Converts the selected Instruments into a single string
    public string InstrumentsToString()
    {
        string tmp = "[";

        for (int i = 0; i< instruments.Length; i++)
        {
            tmp += instruments[i];
        }

        return tmp + "]";
    }

    //Creates the Lobby
    void Open()
    {
        playerName = PlayerName.text;

        StartCoroutine(OpenRequest());
    }

    //Checks if open Lobbys can be found
    IEnumerator CheckforOpenLobbys()
    {
        RequestAnswer requestAnswer = GetComponent<RequestAnswer>();
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/CreateLobby.php", "PassWD=" + "1MRf!s13" + "&Id=" + LobbyID + "&AmountofPlayer=" + amountOfPlayers + "&LobbyKey=" + LobbyKey.ToString() + "&Ispublic=" + isPublic + "&Timestart=" + 0 + "&Instruments=" + InstrumentsToString(), gameObject);

        //Wait for Message
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
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(json);

            //Add Lobbys to UI Element
            for (int i = 0; i< LobbyInfo.Length; i++)
            {
                if (LobbyInfo[i].lobbyKey == LobbyKey)
                {
                    LobbyID = LobbyInfo[i].id;
                    networkManager.lobbyKey = LobbyKey;
                    Open();
                    break;
                }
            }    
        }
        else
        {
            ErrorText.text = "Timeout :(";
            CreateGame.interactable = true;
        }
    }

    //Creates the Lobby via Post Request
    IEnumerator OpenRequest()
    {
        //Creates initial Lobby
        Debug.Log("LK" + LobbyKey);
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/CreateLobbyTabel.php", "PassWD=" + "1" + "&LobbyId=" + LobbyID, gameObject);
        yield return new WaitForSecondsRealtime(1);

        //Adds Players to created Lobby
        Debug.Log("Post to AddPlayers");
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/AddPlayerToLobby.php", "PassWD=" + "1" + "&PlayerName=" + playerName + "&LobbyId=" + LobbyID , gameObject);

        gameObject.SetActive(false);
        WaitingRoom.SetActive(true);

        //Sets Variables for WaitingRoom
        WaitingRoom waitingRoom = WaitingRoom.GetComponent<WaitingRoom>();
        waitingRoom.LobbyID = LobbyID;
        waitingRoom.amountOfPlayers = amountOfPlayers;
        waitingRoom.LobbyKey = LobbyKey;

        networkManager.name = playerName;
        networkManager.lobbyID = LobbyID;
    }
}
