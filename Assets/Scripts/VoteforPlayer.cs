using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
public class VoteforPlayer : NetworkBehaviour
{

    public GameObject playernametext, playerscoretext;
    public GameObject[] players;
    public Button tmpbutton;

    public string playername;
    public float playerscore;

    public static bool voteed;

    public void Update()
    {
        playernametext.GetComponent<TextMeshProUGUI>().text = playername;
        players = GameObject.FindGameObjectsWithTag("Player");
        Checkplayerscore();
        playerscoretext.GetComponent<TextMeshProUGUI>().text = "Votes: " + playerscore.ToString();
        if(VoteforPlayer.voteed == true)
        {
            tmpbutton.interactable = false;
        }
    }

    public void Checkplayerscore()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].name == playername)
            {
                playerscore = players[i].GetComponent<OnlinePlayerController>().playerscore;
            }
        }
    }

    public void Vote()
    {
        if (VoteforPlayer.voteed == true)
        {
            return;
        }
        else
        {
            voteed = true;
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].name == playername)
            {
                players[i].GetComponent<OnlinePlayerController>().playerscore++;
            }
        }
    }
    
}
