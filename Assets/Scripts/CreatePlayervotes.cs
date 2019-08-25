using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreatePlayervotes : NetworkBehaviour
{
    public GameObject playervote_obj, playervoteparent;
    bool allplayersspawned;
    float offset;
    public GameObject[] players;

    public void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (this.gameObject.activeSelf == true)
        {
            for(int i = 0; i < players.Length; i++)
            {
                if(allplayersspawned == false)
                {
                    playervoteparent.transform.GetChild(i).GetComponent<VoteforPlayer>().playername = players[i].name;
                }

                //CmdSpawnPlayervotes(players[i].name);

            }
            allplayersspawned = true;
        }
    }

    [Command]
    public void CmdSpawnPlayervotes(string name)
    {
        if(allplayersspawned == true)
        {
            return;
        }
    }

}
