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
                CmdSpawnPlayervotes(players[i].name);
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

        GameObject playervote = (GameObject)Instantiate(playervote_obj, playervoteparent.transform);
        playervote.transform.localPosition = new Vector3(playervote.transform.localPosition.x,playervote.transform.localPosition.y - offset - 100, 0);
        offset += 105;
        playervote.GetComponent<VoteforPlayer>().playername = name;
        NetworkServer.Spawn(playervote);
    }

}
