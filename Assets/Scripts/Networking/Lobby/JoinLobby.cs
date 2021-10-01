using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(RequestAnswer))]
public class JoinLobby : MonoBehaviour
{
    [Header("References")]
    public GameObject WaitingRoom;
    public RequestManager requestManager;
    NetworkManager networkManager;
    RequestAnswer requestAnswer;

    string lobbyKey;
    string playerName;
    int lobbyID;
    int amountOfPlayers;

    [Header("UI")]
    public TMP_InputField LobbyKeyInput;
    public TMP_InputField PlayerName;
    public TextMeshProUGUI ErrorText;
    public GameObject ContentObj;
    public GameObject SelectionLobbyPrefab;

    [Header("Values")]
    public int maxAmountOfLobbysToDisplay = 10;

    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        requestAnswer = GetComponent<RequestAnswer>();

        LoadPlayerName();
    }

    //Joins a lobby
    public void Join()
    {
        WaitingRoom.SetActive(true);
        gameObject.SetActive(false);

        WaitingRoom waitingRoom = WaitingRoom.GetComponent<WaitingRoom>();
        waitingRoom.LobbyID = lobbyID;
        waitingRoom.LobbyKey = lobbyKey;
        waitingRoom.amountOfPlayers = amountOfPlayers;

        playerName = PlayerName.text;
        networkManager.name = PlayerName.text;
        networkManager.lobbyID = lobbyID;
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/AddPlayerToLobby.php", "PassWD=" + "1" + "&PlayerName=" + playerName + "&LobbyId=" + lobbyID, gameObject);
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
    public void CheckLobby()
    {
        if (LobbyKeyInput.text.Length <= 4)
        {
            try
            {
                int.Parse(LobbyKeyInput.text);
            }
            catch (Exception)
            {
                ErrorText.text = "Please enter a Lobbykey";
            }

            return;
        }
        else if (PlayerName.text.Length <= 1)
        {
            ErrorText.text = "Please enter a Name";
            return;
        }

        lobbyKey = LobbyKeyInput.text;
        StartCoroutine(Check());
    }

    //Displays all open Lobbys
    public void DisplayAllOpenLobbys()
    {
        StartCoroutine(DisplayAllOpenLobbyInner());
    }

    //Checks if lobby can be found
    public IEnumerator Check()
    {
        requestAnswer.Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php", "PassWD=" + "1MRf!s13", gameObject);

        //Wait for Message
        float time = 0;
        while (requestAnswer.Message.Length <  1)
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
            //Get Lobbydata
            string json = "{\"Items\":" + requestAnswer.Message + "}";
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(json);

            for (int i = 0; i < LobbyInfo.Length; i++)
            {
                Debug.Log(int.Parse(LobbyKeyInput.text));
                Debug.Log(int.Parse(LobbyInfo[i].lobbyKey));

                if (int.Parse(LobbyInfo[i].lobbyKey) == int.Parse(LobbyKeyInput.text))
                {
                    lobbyID = LobbyInfo[i].id;
                    networkManager.lobbyKey = lobbyKey;
                    amountOfPlayers = int.Parse(LobbyInfo[i].amountOfPlayers);
                    StartCoroutine(UpdatePlayers());
                    break;
                }
            }
        }
        else
        {
            ErrorText.text = "Something went wrong :(";
        }
    }

    //Checks if lobby can be joined
    public IEnumerator UpdatePlayers()
    {
        requestAnswer.Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + lobbyID, gameObject);

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

        Debug.Log("M: " + requestAnswer.Message);

        //If Message received
        if (requestAnswer.Message.Length > 1)
        {
            string json = "{\"Items\":" + requestAnswer.Message + "}";
            PlayerInfo[] PlayerInfo = JsonHelper.FromJson<PlayerInfo>(json);
            bool nameIsUsed = false;

            if (PlayerInfo.Length < amountOfPlayers)
            {
                //Checks if the Playername is already used - if not, join
                for (int i = 0; i < PlayerInfo.Length; i++)
                {
                    Debug.Log("Playername: " + PlayerInfo[i].PlayerName);
                    if (PlayerName.text.ToString().Trim() == PlayerInfo[i].PlayerName)
                    {
                        ErrorText.text = "Name is Allready Used!";
                        nameIsUsed = true;
                    }

                    if (i == PlayerInfo.Length - 1 && !nameIsUsed)
                    {
                        Join();
                    }
                }

                //If lobby is empty, join
                if (PlayerInfo.Length == 0)
                {
                    Join();
                }
            }
            else
            {
                ErrorText.text = "Lobby is full :(";
            }
        }
        else
        {
            ErrorText.text = "Timeout :(";
        }
    }

    //Displays all open Lobbys via Post Request
    public IEnumerator DisplayAllOpenLobbyInner()
    {
        requestAnswer.Message = "";
        yield return new WaitForSecondsRealtime(Time.deltaTime);

        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php", "PassWD=" + "1MRf!s13", gameObject);

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
            //Get Lobby Data
            string json = "{\"Items\":" + requestAnswer.Message + "}";
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(json);
            GenerateListOfLobbys(LobbyInfo);
        }
        else
        {
            ErrorText.text = "Something went wrong :(";
        }
    }

    //Fills UI List with Lobbys
    public void GenerateListOfLobbys(LobbyInfo[] LobbyInfo)
    {
        int counter = 1;

        //Clear initial List
        foreach (Transform child in ContentObj.transform)
        {
            Destroy(child.gameObject);
        }

        //Adds Lobbys to List
        for (int i = 0; i < LobbyInfo.Length; i++)
        {
            if (i == maxAmountOfLobbysToDisplay)
            {
                return;
            }
            else if (LobbyInfo[i].isPublic != "True" || int.Parse(LobbyInfo[i].startTime) > networkManager.timeOut)
            {
                continue;
            }

            GameObject go = Instantiate(SelectionLobbyPrefab, ContentObj.transform);
            ContentObj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, counter * 100);
            go.GetComponent<SelectLobby>().LobbyKey = LobbyInfo[i].lobbyKey;
            go.GetComponent<SelectLobby>().message = "Max: " + LobbyInfo[i].amountOfPlayers;
            counter++;
        }
    }

    //Sets the Lobbykey
    public void SetLobbyKey(string lobbykey)
    {
        lobbyKey = lobbykey;
        LobbyKeyInput.text = lobbykey;
    }
}
