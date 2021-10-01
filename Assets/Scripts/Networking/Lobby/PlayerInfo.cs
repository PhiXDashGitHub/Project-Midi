using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string PlayerName;
    public string Score;
    public string Notes;
    public string BPM;
    public string Reverb;
    public string Volume;
    public string Ready;
    public string VotingReady;

    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString);
    }
}
