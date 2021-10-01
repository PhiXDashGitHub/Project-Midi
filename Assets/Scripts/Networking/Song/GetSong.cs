using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RequestAnswer))]
public class GetSong : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;
    RequestAnswer requestAnswer;

    public Rating rating;
    public NoteEditor noteEditor;

    public PlayerInfo[] playerinfo = new PlayerInfo[0];

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        requestAnswer = GetComponent<RequestAnswer>();
    }

    //Play Song
    public void Play(string player)
    {
        StartCoroutine(PlayInner(player));
    }

    //Init Song Playback
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
                playerinfo = JsonHelper.FromJson<PlayerInfo>(json);

                //Find the players song and play it
                for (int i = 0; i < playerinfo.Length; i++)
                {
                    if (playerinfo[i].PlayerName == player)
                    {
                        //Play Song
                        noteEditor.songLength = 0;
                        noteEditor.StringToNoteData(playerinfo[i].Notes);
                        noteEditor.StringToReverb(playerinfo[i].Reverb);
                        noteEditor.StringToVolume(playerinfo[i].Volume);
                        noteEditor.bpm = int.Parse(playerinfo[i].BPM);

                        noteEditor.PlaySoundData();
                        StartCoroutine(WaitforSongEnd());
                    }
                }
            }
        }
    }

    //Waits until Song Playback finished
    public IEnumerator WaitforSongEnd()
    {
        yield return new WaitForSecondsRealtime(noteEditor.songLength + 5);

        Debug.Log("Setting Songend true");
        rating.songEnd = true;
    }
}
