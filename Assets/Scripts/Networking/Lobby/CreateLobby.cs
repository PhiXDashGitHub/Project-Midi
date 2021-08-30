using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RequestAnswer))]
public class CreateLobby : MonoBehaviour
{
    [Header("Networking")]
    public RequestManager requestManager;
    NetworkManager networkManager;
    string Httprequest;


    bool isprivate;
    string LobbyKey;
    string Playername;
    int AmountofPlayers = 5;
    int LobbyID;

    public int LobbyKeyLeght;

    [Header("UI")]
    public TextMeshProUGUI LobbyKeyText;
    public TextMeshProUGUI PrivateText;
    public GameObject WaitingRoom;
    public TMP_InputField PlayerName;

    public TextMeshProUGUI ErrorText;
    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        CreateLobbyKey();
        SetLobbyState();
    }

    public void Checkforopen()
    {
        if (PlayerName.text.Length < 1)
        {
            ErrorText.text = "Please enter a Name";
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
        isprivate = !isprivate;
        if (isprivate)
        {
            PrivateText.text = "Game is private";
        }
        else
        {
            PrivateText.text = "Game is open";
        }
    }

    public void SetAmountofPlayers(int i) 
    {
        AmountofPlayers = i;
    }


    IEnumerator CheckforOpenLobbys()
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php","PassWD=" + "1MRf!s13", this.gameObject);
        
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
                if (int.Parse(LobbyInfo[i].Timestart) > 14)
                {
                    LobbyID = LobbyInfo[i].Id;
                    Open();
                    break;
                }
            }
            ErrorText.text = "No Open Lobby Found";
        }
        else
        {
            ErrorText.text = "Ups :(";
        }
    }


    void Open()
    {
        Playername = PlayerName.text;
        Debug.Log("LK" + LobbyKey);
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/CreateLobby.php", "PassWD=" + "1MRf!s13" + "&Id=" + LobbyID + "&AmountofPlayer=" + AmountofPlayers + "&LobbyKey=" + LobbyKey.ToString() + "&Ispublic="+ isprivate + "&Timestart=" + 0, this.gameObject);

        Debug.Log("Post to ClearOldPlayers");
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/ClearAllPlayers.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + LobbyID, this.gameObject);

        Debug.Log("Post to AddPlayers");
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/AddPlayerToLobby.php", "PassWD=" + "1" + "&PlayerName=" + Playername + "&LobbyId=" + LobbyID, this.gameObject);
        
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
        public string AmountofPlayer;
        public string Timestart;

        public static LobbyInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<LobbyInfo>(jsonString);
        }
    }
}
