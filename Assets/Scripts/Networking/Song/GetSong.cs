using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RequestAnswer))]
public class GetSong : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;

    public Rating rating;
    public NoteEditor noteEditor;

    public PlayerInfo[] playerinfo = new PlayerInfo[0];

    void Start()
    {
        
    }

    public void Play(string player)
    {

        StartCoroutine(PlayInner(player));
    }

    public IEnumerator PlayInner(string player)
    {
        if (playerinfo.Length > 1)
        {
            for (int i = 0; i < playerinfo.Length; i++)
            {
                if (playerinfo[i].PlayerName == player)
                {
                    //PlaySong
                    noteEditor.StringToNoteData(playerinfo[i].Notes);
                    noteEditor.PlaySoundData();
                    StartCoroutine(WaitforSongEnd());
                }
            }
        }
        else
        {
            networkManager = FindObjectOfType<NetworkManager>();
            requestManager = FindObjectOfType<RequestManager>();
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
                playerinfo = JsonHelper.FromJson<PlayerInfo>(josn);

                for (int i = 0; i < playerinfo.Length; i++)
                {
                    if (playerinfo[i].PlayerName == player)
                    {
                        //PlaySong
                        noteEditor.SongLenght = 0;
                        noteEditor.StringToNoteData(playerinfo[i].Notes);
                        noteEditor.bpm = int.Parse(playerinfo[i].BPM);
                        noteEditor.PlaySoundData();
                        StartCoroutine(WaitforSongEnd());
                    }
                }
            }
        }
    }

    public IEnumerator WaitforSongEnd()
    {
        yield return new WaitForSeconds(noteEditor.SongLenght);
        yield return new WaitForSeconds(5);
        Debug.Log("Setting Songend true");
        rating.songend = true;
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string PlayerName;
        public string Score;
        public string Notes;
        public string BPM;
        public static PlayerInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PlayerInfo>(jsonString);
        }
    }
}
