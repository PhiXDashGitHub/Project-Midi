using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInfo
{
    public int id;
    public string name;
    public string amountOfPlayers;
    public string lobbyKey;
    public string startTime;
    public string isPublic;

    public static LobbyInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LobbyInfo>(jsonString);
    }
}
