using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(RequestAnswer))]
public class CreateLobby : MonoBehaviour
{
    [Header("Networking")]
    public RequestManager requestManager;
    NetworkManager networkManager;
    string Httprequest;


    bool Ispublic;
    string LobbyKey;
    string Playername;
    int AmountofPlayers = 5;
    int LobbyID;

    public string[] Instruments;

    public int LobbyKeyLeght;

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

        Instruments = new string[InstrumentSelection.MaxamountofInstruments];

        LoadPlayerName();
    }

    public void SavePlayerName()
    {
        PlayerPrefs.SetString("PlayerName", PlayerName.text);
    }

    public void LoadPlayerName()
    {
        PlayerName.text = PlayerPrefs.GetString("PlayerName");
    }

    public void Checkforopen()
    {
        if (PlayerName.text.Length < 1)
        {
            ErrorText.text = "Please enter a Name";
            CreateGame.interactable = true;
            return;
        }
        if (Instruments.Length < 1)
        {
            ErrorText.text = "Please Select a Instrument";
            CreateGame.interactable = true;
            return;
        }
        StartCoroutine(CheckforOpenLobbys());
    }
    
    public void CreateLobbyKey()
    {
        for (int i = 0; i< LobbyKeyLeght; i++)
        {
            LobbyKey += Random.Range(0, 9).ToString();
        }
        LobbyKeyText.text = LobbyKey;
    }

    public void SetLobbyState()
    {
        Ispublic = !Ispublic;
        if (!Ispublic)
        {
            PrivateText.text = "Game is private";
        }
        else
        {
            PrivateText.text = "Game is open";
        }
        Debug.Log("Bool: " + Ispublic);
    }

    public void SetAmountofPlayers(int i) 
    {
        AmountofPlayers = i;
    }

    public string InstrumentsToString()
    {
        string tmp = "[";

        for (int i = 0; i< Instruments.Length; i++)
        {
            tmp += Instruments[i];
        }
        tmp += "]";
        return tmp;
    }

    IEnumerator CheckforOpenLobbys()
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/CreateLobby.php", "PassWD=" + "1MRf!s13" + "&Id=" + LobbyID + "&AmountofPlayer=" + AmountofPlayers + "&LobbyKey=" + LobbyKey.ToString() + "&Ispublic=" + Ispublic + "&Timestart=" + 0 + "&Instruments=" + InstrumentsToString(), this.gameObject);

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
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(josn);
            for (int i = 0; i< LobbyInfo.Length; i++)
            {
                if (LobbyInfo[i].LobbyKey == LobbyKey)
                {
                    LobbyID = LobbyInfo[i].Id;
                    networkManager.LobbyKey = LobbyKey;
                    Open();
                    break;
                }
            }
            
        }
        else
        {
            ErrorText.text = "Ups :(";
            CreateGame.interactable = true;
        }
    }


    void Open()
    {
        Playername = PlayerName.text;

        StartCoroutine(Openrequestdelay());
    }
    public IEnumerator Openrequestdelay()
    {
        Debug.Log("LK" + LobbyKey);
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/CreateLobbyTabel.php", "PassWD=" + "1" + "&LobbyId=" + LobbyID, this.gameObject);
        yield return new WaitForSeconds(1);
        Debug.Log("Post to AddPlayers");
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/AddPlayerToLobby.php", "PassWD=" + "1" + "&PlayerName=" + Playername + "&LobbyId=" + LobbyID , this.gameObject);
        WaitingRoom.SetActive(true);
        this.gameObject.SetActive(false);
        WaitingRoom.GetComponent<WaitingRoom>().LobbyID = LobbyID;
        WaitingRoom.GetComponent<WaitingRoom>().amoutofplayers = AmountofPlayers;
        WaitingRoom.GetComponent<WaitingRoom>().LobbyKey = LobbyKey;

        networkManager.Name = Playername;
        networkManager.LobbyID = LobbyID;
    }
   
    [System.Serializable]
    public class LobbyInfo
    {
        public int Id;
        public string Name;
        public string LobbyKey;
        public string AmountofPlayer;
        public string Timestart;

        public static LobbyInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<LobbyInfo>(jsonString);
        }
    }
}
