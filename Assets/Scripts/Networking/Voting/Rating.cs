using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
[RequireComponent(typeof(RequestAnswer))]
public class Rating : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;
    RequestAnswer requestAnswer;

    int playerIndex;
    int playerIndexVoted = -2;
    int amountOfTrys;
    [HideInInspector] public bool songEnd = false;

    List<string> players = new List<string>();

    [Header("RatingScreen UI")]
    public GameObject VoteScreen;
    public TextMeshProUGUI PlayerNameText;
    public GetSong getSong;

    [Header("WinnScreen UI")]
    public GameObject WinScreen;
    public TextMeshProUGUI WinnerText;
    public TextMeshProUGUI PlaceText2;
    public TextMeshProUGUI PlaceText3;
    public TextMeshProUGUI PlaceText4;
    public GameObject Buttons;
    public TextMeshProUGUI DebugText;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        requestAnswer = GetComponent<RequestAnswer>();

        if (networkManager)
        {
            players = networkManager.players;
            playerIndex = 0;

            GetAllPlayerStats();
        }
        else
        {
            Debug.LogError("NetworkManager not found.");
        }
    }

    public void Update()
    {
        if (songEnd)
        {
            Buttons.SetActive(true);
        }
        else
        {
            Buttons.SetActive(false);
        }
    }

    //Loads a new Player
    public void LoadNewPlayer()
    {
        if (players[playerIndex] == networkManager.name)
        {
            playerIndex++;
            LoadNewPlayer();
            return;
        }

        Debug.Log("Index Playindex: " + playerIndex);
        PlayerNameText.text = "Currently Playing: " + players[playerIndex];
        getSong.Play(players[playerIndex]);
        songEnd = false;
        playerIndex++;
    }

    //Vote a Players Song
    public void Vote(int i)
    {
        if (playerIndexVoted != playerIndex)
        {
            StartCoroutine(VoteInner(i));

            playerIndexVoted = playerIndex;
            LoadNewPlayer();
        }
    }

    //Display Win Screen
    public void DisplayWinners()
    {
        StartCoroutine(SendReadyInner());
    }

    //Gets all Players Stats
    public void GetAllPlayerStats()
    {
        StartCoroutine(GetAllPlayerStatsInner());
    }

    //Vote a Players Song via Post Request
    IEnumerator VoteInner(int i)
    {
        playerIndexVoted = playerIndex;
        requestAnswer.Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendScore.php", "PassWD=" + "1" + "&PlayerName=" + players[playerIndex - 1] + "&LobbyId=" + networkManager.lobbyID + "&Score=" + i, gameObject);

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
            Debug.Log("Vote Worked!");
            if (playerIndexVoted == players.Count)
            {
                DisplayWinners();
                songEnd = false;
            }
        }
    }

    //Gets all Votings via Post Request
    IEnumerator GetAllVotings()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        requestAnswer.Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + networkManager.lobbyID, gameObject);

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
            PlayerInfo[] PlayerInfo = JsonHelper.FromJson<PlayerInfo>(json);

            //Get PlayerReady Infos
            int playersReady = 0;
            for (int i = 0; i < PlayerInfo.Length; i++)
            {
                if (PlayerInfo[i].VotingReady == "true")
                {
                    playersReady++;
                }
            }

            if (playersReady == PlayerInfo.Length)
            {
                //All Players are Ready
                Debug.Log("All Players Voting Ready");
                StartCoroutine(DisplayWinnersInner());
            }
            else
            {
                //Try again 80 times - else display all Winners
                if (amountOfTrys < 80)
                {
                    amountOfTrys++;
                    StartCoroutine(GetAllVotings());
                }
                else
                {
                    StartCoroutine(DisplayWinnersInner());
                }
            }
        }
    }

    //Sets voting results via Post Request
    IEnumerator SendReadyInner()
    {
        Debug.Log("Send Inner");

        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendReady.php", "PassWD=" + "1" + "&PlayerName=" + networkManager.name + "&LobbyId=" + networkManager.lobbyID + "&VotingReady=true", gameObject);

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
            StartCoroutine(GetAllVotings());
        }
    }

    //Displays all Players via Post Request
    IEnumerator DisplayWinnersInner()
    {
        WinScreen.SetActive(true);
        VoteScreen.SetActive(false);

        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + networkManager.lobbyID, gameObject);

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
            PlayerInfo[] PlayerInfo = JsonHelper.FromJson<PlayerInfo>(json);
            int[] sortedScore = new int[PlayerInfo.Length];
            List<string> ShownPlayers = new List<string>();

            for (int i = 0; i < PlayerInfo.Length; i++)
            {
                sortedScore[i] = int.Parse(PlayerInfo[i].Score);
            }

            Array.Sort(sortedScore);
            Array.Reverse(sortedScore);

            for (int i = 0; i< sortedScore.Length; i++)
            {
                for (int j = 0; j < PlayerInfo.Length; j++)
                {
                    if (sortedScore[i] == int.Parse(PlayerInfo[j].Score))
                    {
                        bool notdisplayed = true;

                        for (int k = 0; k < ShownPlayers.Count; k++)
                        {
                            if (ShownPlayers[k] == PlayerInfo[j].PlayerName)
                            {
                                notdisplayed = false;
                                break;
                            }
                        }

                        //UI Text for all Players and their Scores
                        if (notdisplayed) 
                        {
                            if (i == 0 && WinnerText.text.Length < 2)
                            {
                                WinnerText.text = "The Winner is: " + PlayerInfo[j].PlayerName + "\n" + "Score: " + PlayerInfo[j].Score;
                                ShownPlayers.Add(PlayerInfo[j].PlayerName);
                            }
                            else if (i == 1 && PlaceText2.text.Length < 2)
                            {
                                PlaceText2.text += i + 1 + " Place: " + PlayerInfo[j].PlayerName + " Score: " + PlayerInfo[j].Score;
                                ShownPlayers.Add(PlayerInfo[j].PlayerName);
                            }
                            else if (i == 2 && PlaceText3.text.Length < 2)
                            {
                                PlaceText3.text += i + 1 + " Place: " + PlayerInfo[j].PlayerName + " Score: " + PlayerInfo[j].Score;
                                ShownPlayers.Add(PlayerInfo[j].PlayerName);
                            }
                            else if (i == 3 && PlaceText4.text.Length < 2)
                            {
                                PlaceText4.text += i + 1 + " Place: " + PlayerInfo[j].PlayerName + " Score: " + PlayerInfo[j].Score;
                                ShownPlayers.Add(PlayerInfo[j].PlayerName);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }

    //Gets all Players Stats via Post Request
    IEnumerator GetAllPlayerStatsInner()
    {
        yield return new WaitForSecondsRealtime(3);
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetPlayerData.php", "PassWD=" + "1MRf!s13" + "&LobbyId=" + networkManager.lobbyID, gameObject);

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
        if (requestAnswer.Message.Length > 2)
        {
            string json = "{\"Items\":" + requestAnswer.Message + "}";
            PlayerInfo[] PlayerInfo = JsonHelper.FromJson<PlayerInfo>(json);
            
            //Get PlayerReady Infos
            int playersReady = 0;
            for (int i = 0; i < PlayerInfo.Length; i ++)
            {
                if (PlayerInfo[i].Ready == "true")
                {
                    playersReady++;
                }
            }

            //If all Players are ready
            if (playersReady == PlayerInfo.Length)
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
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
