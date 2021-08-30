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


    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
    }

    public void Play(string player)
    {
        StartCoroutine(PlayInner(player));
    }

    public IEnumerator PlayInner(string player)
    {
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

            for (int i = 0; i < PlayerInfo.Length; i++)
            {
                if (PlayerInfo[i].PlayerName == player)
                {
                    //PlaySong
                    noteEditor.StringToNoteData(PlayerInfo[i].Song);
                    noteEditor.PlaySoundData();
                    StartCoroutine(WaitforSongEnd());
                }
            }
        }
    }

    public IEnumerator WaitforSongEnd()
    {
        yield return new WaitForSeconds(noteEditor.SongLenght);
        rating.LoadNewPlayer();
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string PlayerName;
        public string Score;
        public string Song;

        public static PlayerInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PlayerInfo>(jsonString);
        }
    }
}
