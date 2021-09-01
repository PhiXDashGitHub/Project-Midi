using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RequestAnswer))]
public class SendSong : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;

    public int bpm;
    public float volume;
    public float reverb;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
    }

    public void Send(string song)
    {
        StartCoroutine(SendInner(song));
    }

    public IEnumerator SendInner(string song)
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendSong.php", "PassWD=" + "1" + "&PlayerName=" + networkManager.Name + "&Song=" + song + "&LobbyId=" + networkManager.LobbyID + "&BPM=" + bpm + "&Ready=true" + "&Reverb=(0,0,0,0,0,0)" + "&Volume=(0,0,0,0,0,0)", this.gameObject);

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
            Debug.Log("Song Updated");
        }
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
