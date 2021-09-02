using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof(RequestAnswer))]
public class Rating : MonoBehaviour
{

    RequestManager requestManager;
    NetworkManager networkManager;


    int playerindex;
    int tmpplayerindexvoted = -2;
    [HideInInspector]
    public bool songend = false;

    List<string> players = new List<string>();


    [Header("RatingScreen UI")]
    public GameObject VoteScreen;
    public TextMeshProUGUI PlayerNameText;
    public GetSong getSong;

    [Header("WinnScreen UI")]
    public GameObject WinScreen;
    public TextMeshProUGUI WinnerText;
    public TextMeshProUGUI PlayerListText;
    public GameObject Buttons;

    public TextMeshProUGUI DebugText;
    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();

        if (networkManager)
        {
            players = networkManager.players;
            playerindex = 0;
            GetAllPlayerStats();
        }
        else
        {
            Debug.LogError("NetworkManager not found.");
        }
    }

    public void LoadNewPlayer()
    {
        if (playerindex == players.Count)
        {
            DisplayWinners();
            songend = false;
            return;
        }
        else
        {
            if (players[playerindex] == networkManager.Name)
            {
                playerindex++;
                LoadNewPlayer();
                return;
            }
        }
        Debug.Log("Index Playindex: " + playerindex);
        PlayerNameText.text = "Currently Playing: " + players[playerindex];
        getSong.Play(players[playerindex]);
        songend = false;
        playerindex++;
    }

    public void Update()
    {
        if (songend)
        {
            Buttons.SetActive(true);
        }
        else
        {
            Buttons.SetActive(false);
        }
    }

    public void Vote(int i)
    {
        if (songend == false)
        {
            return;
        }
        
        if (playerindex == players.Count)
        {
            LoadNewPlayer();
            return;
        }

        if (tmpplayerindexvoted != playerindex)
        {
            Debug.Log("Vote before Coroutine Working");
            StartCoroutine(VoteInner(i));
            
            tmpplayerindexvoted = playerindex;
            LoadNewPlayer();
        }
    }

    public IEnumerator VoteInner(int i)
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendScore.php", "PassWD=" + "1" + "&PlayerName=" + players[playerindex] + "&LobbyId=" + networkManager.LobbyID + "&Score=" + i, this.gameObject);

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
            Debug.Log("Vote Worked!");
        }
    }

    public void DisplayWinners()
    {
        StartCoroutine(SendReadyInner());   
    }

    public IEnumerator GetAllVotings()
    {
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<RequestAnswer>().Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + networkManager.LobbyID, this.gameObject);

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
            int playersready = 0;
            for (int i = 0; i < PlayerInfo.Length; i++)
            {
                if (PlayerInfo[i].VotingReady == "true")
                {
                    playersready++;
                }
            }

            if (playersready == PlayerInfo.Length)
            {
                //All Players are Ready
                Debug.Log("All Players Voting Ready");
                StartCoroutine(DisplayWinnersInner());
            }
            else
            {
                StartCoroutine(GetAllVotings());
            }
        }
    }

    public IEnumerator SendReadyInner()
    {
        Debug.Log("Send Inner");

        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendReady.php", "PassWD=" + "1" + "&PlayerName=" + networkManager.Name + "&LobbyId=" + networkManager.LobbyID + "&VotingReady=true", this.gameObject);

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
            StartCoroutine(GetAllVotings());
        }
    }

    public IEnumerator DisplayWinnersInner()
    {
        WinScreen.SetActive(true);
        VoteScreen.SetActive(false);

        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + networkManager.LobbyID, this.gameObject);

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
            int[] sortscore = new int[PlayerInfo.Length];

            for (int i = 0; i < PlayerInfo.Length; i++)
            {
                sortscore[i] = int.Parse(PlayerInfo[i].Score);
            }

            Array.Sort(sortscore);
            Array.Reverse(sortscore);

            for (int i = 0; i< sortscore.Length; i++)
            {
                for (int j = 0; j < PlayerInfo.Length; j++)
                {
                    if (sortscore[i] == int.Parse(PlayerInfo[j].Score))
                    {
                        if (i == 0)
                        {
                            WinnerText.text = "The Winner is: " + PlayerInfo[j].PlayerName +"\n" + "Score: " + PlayerInfo[j].Score;
                        }
                        else
                        {
                            PlayerListText.text += i+1 + " Place: " + PlayerInfo[j].PlayerName + "Score: " + PlayerInfo[j].Score + "\n";
                        }
                        break;
                    }
                }
            }
        }
    }

    public void GetAllPlayerStats()
    {
        StartCoroutine(GetAllPlayerStatsInner());
    }

    public IEnumerator GetAllPlayerStatsInner()
    {
        yield return new WaitForSeconds(3);
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + networkManager.LobbyID, this.gameObject);

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
            int playersready = 0;
            for (int i = 0; i < PlayerInfo.Length; i ++)
            {
                if (PlayerInfo[i].Ready == "true")
                {
                    playersready++;
                }
            }

            if (playersready == PlayerInfo.Length)
            {
                //All Players are Ready
                Debug.Log("All Players Ready");
                LoadNewPlayer();
            }
            else
            {
                StartCoroutine(GetAllPlayerStatsInner());
            }
        }
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string PlayerName;
        public string Score;
        public string Ready;
        public string VotingReady;

        public static PlayerInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PlayerInfo>(jsonString);
        }
    }
}
