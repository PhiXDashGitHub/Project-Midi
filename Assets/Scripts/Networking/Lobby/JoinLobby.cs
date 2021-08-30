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

    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void Join()
    {
        WaitingRoom.SetActive(true);
        WaitingRoom.GetComponent<WaitingRoom>().LobbyID = LobbyID;
        WaitingRoom.GetComponent<WaitingRoom>().amoutofplayers = AmountofPlayers;
        Playername = PlayerName.text;
        networkManager.Name = PlayerName.text;
        networkManager.LobbyID = LobbyID;
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/AddPlayerToLobby.php", "PassWD=" + "1" + "&PlayerName=" + Playername + "&LobbyId=" + LobbyID, this.gameObject);
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
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php", "PassWD=" + "1MRf!s13", this.gameObject);

        float time = 0;
        while (this.GetComponent<RequestAnswer>().Message.Length <  1)
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
                    AmountofPlayers = int.Parse(LobbyInfo[i].AmountofPlayer);
                    Join();
                    break;
                }
            }
            ErrorText.text = "No Open Lobby Found";
            
        }
        else
        {
            ErrorText.text = "Something went wrong :(";
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

        public static LobbyInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<LobbyInfo>(jsonString);
        }
    }
}
