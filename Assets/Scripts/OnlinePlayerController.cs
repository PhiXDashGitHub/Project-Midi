using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlinePlayerController : NetworkBehaviour
{
    [SyncVar]
    public bool GameEnd;
    public float playerscore;
    public string playername;

    public GameObject Endscreen;

    void Update()
    {
        this.gameObject.name = playername;
        if (!isLocalPlayer)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            CmdSendNameToServer(playername);
            CmdSendScoreToServer(playerscore);
        }
        if(GameEnd == true)
        {
            Endscreen.SetActive(true);
            //this.gameObject.SetActive(false);
        }
    }

    public void DisplayEndScreen()
    {
        GameEnd = true;
    }

    [Command]
    void CmdSendNameToServer(string nameToSend)
    {
        RpcSetPlayerName(nameToSend);
    }

    [ClientRpc]
    void RpcSetPlayerName(string name)
    {
        playername = name;
    }
    [Command]
    void CmdSendScoreToServer(float score)
    {
        RpcSetPlayerScore(score);
    }

    [ClientRpc]
    void RpcSetPlayerScore(float score)
    {
        playerscore = score;
    }
}
