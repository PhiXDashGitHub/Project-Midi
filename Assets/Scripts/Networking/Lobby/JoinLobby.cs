using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(RequestAnswer))]
public class JoinLobby : MonoBehaviour
{
    [Header("Networking")]
    public RequestManager requestManager;
    NetworkManager networkManager;
    public GameObject WaitingRoom;

    string LobbyKey;
    string Playername;
    int LobbyID;
    int AmountofPlayers;
    [Header("UI")]
    public TMP_InputField LobbyKeyInput;
    public TMP_InputField PlayerName;
    public TextMeshProUGUI ErrorText;
    public GameObject ContentObj;
    public GameObject SelectionLobbyPrefab;

    [Header("Values")]
    public int MaxamountofLobbysToDisplay;

    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        LoadPlayerName();
    }

    public void Join()
    {
        WaitingRoom.SetActive(true);
        this.gameObject.SetActive(false);
        WaitingRoom.GetComponent<WaitingRoom>().LobbyID = LobbyID;
        WaitingRoom.GetComponent<WaitingRoom>().LobbyKey = LobbyKey;
        WaitingRoom.GetComponent<WaitingRoom>().amoutofplayers = AmountofPlayers;
        Playername = PlayerName.text;
        networkManager.Name = PlayerName.text;
        networkManager.LobbyID = LobbyID;
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/AddPlayerToLobby.php", "PassWD=" + "1" + "&PlayerName=" + Playername + "&LobbyId=" + LobbyID, this.gameObject);
    }

    public void SavePlayerName()
    {
        PlayerPrefs.SetString("PlayerName", PlayerName.text);
    }

    public void LoadPlayerName()
    {
        PlayerName.text = PlayerPrefs.GetString("PlayerName");
    }

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
                ErrorText.text = "Please enter a LobbyKey";
            }
            return;
        }


        if (PlayerName.text.Length <= 1)
        {
            ErrorText.text = "Please enter a Name";
            return;
        }

        LobbyKey = LobbyKeyInput.text;
        StartCoroutine(Check());
    }

    public IEnumerator Check()
    {
        this.GetComponent<RequestAnswer>().Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php", "PassWD=" + "1MRf!s13", this.gameObject);

        float time = 0;
        while (this.GetComponent<RequestAnswer>().Message.Length <  1)
        {
            if (time > networkManager.Timeout)
            {
                break;
            }
            time += Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }

        if (this.GetComponent<RequestAnswer>().Message.Length > 1)
        {
            //Get Lobby Data

            string josn = "{\"Items\":" + this.GetComponent<RequestAnswer>().Message + "}";
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(josn);

            for (int i = 0; i < LobbyInfo.Length; i++)
            {
                Debug.Log(int.Parse(LobbyKeyInput.text));
                Debug.Log(int.Parse(LobbyInfo[i].LobbyKey));

                if (int.Parse(LobbyInfo[i].LobbyKey) == int.Parse(LobbyKeyInput.text))
                {
                    LobbyID = LobbyInfo[i].Id;
                    networkManager.LobbyKey = LobbyKey;
                    AmountofPlayers = int.Parse(LobbyInfo[i].AmountofPlayer);
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

    public IEnumerator UpdatePlayers()
    {
        this.GetComponent<RequestAnswer>().Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + LobbyID, this.gameObject);

        float time = 0;
        while (this.GetComponent<RequestAnswer>().Message.Length < 1)
        {
            if (time > networkManager.Timeout)
            {
                break;
            }
            time += Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        Debug.Log("M: " + this.GetComponent<RequestAnswer>().Message);
        if (this.GetComponent<RequestAnswer>().Message.Length > 1)
        {
            string josn = "{\"Items\":" + this.GetComponent<RequestAnswer>().Message + "}";
            PlayerInfo[] PlayerInfo = JsonHelper.FromJson<PlayerInfo>(josn);
            bool nameisused = false;

            if (PlayerInfo.Length < AmountofPlayers)
            {
                for (int i = 0; i < PlayerInfo.Length; i++)
                {
                    Debug.Log("Playername: " + PlayerInfo[i].PlayerName);
                    if (PlayerName.text.ToString().Trim() == PlayerInfo[i].PlayerName)
                    {
                        ErrorText.text = "Name is Allready Used!";
                        nameisused = true;
                    }

                    if (i == PlayerInfo.Length - 1 && !nameisused)
                    {
                        Join();
                    }
                }
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


    public void DisplayAllOpenLobbys()
    {
        StartCoroutine(DisplayAllOpenLobbyInner());
    }

    public IEnumerator DisplayAllOpenLobbyInner()
    {
        this.GetComponent<RequestAnswer>().Message = "";
        yield return new WaitForSecondsRealtime(Time.deltaTime);
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php", "PassWD=" + "1MRf!s13", this.gameObject);

        float time = 0;
        while (this.GetComponent<RequestAnswer>().Message.Length < 1)
        {
            if (time > networkManager.Timeout)
            {
                break;
            }
            time += Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }

        if (this.GetComponent<RequestAnswer>().Message.Length > 1)
        {
            //Get Lobby Data

            string josn = "{\"Items\":" + this.GetComponent<RequestAnswer>().Message + "}";
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(josn);
            GenerateListOfLobbys(LobbyInfo);
        }
        else
        {
            ErrorText.text = "Something went wrong :(";
        }
    }

    public void GenerateListOfLobbys(LobbyInfo[] LobbyInfo)
    {
        int conter = 1;

        foreach (Transform child in ContentObj.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < LobbyInfo.Length; i++)
        {
            if (i == MaxamountofLobbysToDisplay)
            {
                return;
            }
            if (LobbyInfo[i].Ispublic != "True" || int.Parse(LobbyInfo[i].Timestart) > 5)
            {
                continue;
            }
            GameObject go = Instantiate(SelectionLobbyPrefab, ContentObj.transform);
           // go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (conter * -100) - 50);
            ContentObj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, conter * 100);
            go.GetComponent<SelectLobby>().LobbyKey = LobbyInfo[i].LobbyKey;
            go.GetComponent<SelectLobby>().message = "Max: " + LobbyInfo[i].AmountofPlayer;
            conter++;
        }
    }

    public void SetLobbyKey(string lobbykey)
    {
        LobbyKey = lobbykey;
        LobbyKeyInput.text = lobbykey;
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

    [System.Serializable]
    public class LobbyInfo
    {
        public int Id;
        public string Name;
        public string AmountofPlayer;
        public string LobbyKey;
        public string Timestart;
        public string Ispublic;
        public static LobbyInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<LobbyInfo>(jsonString);
        }
    }
}
