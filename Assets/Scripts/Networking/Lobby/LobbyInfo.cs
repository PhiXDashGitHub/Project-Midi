using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LobbyInfo
{
    public int Id;
    public string Name;
    public string AmountofPlayer;
    public string LobbyKey;
    public int Timestart;
    public string Ispublic;
    public string Instruments;
    public string VoteReady;

    public static LobbyInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LobbyInfo>(jsonString);
    }
}
