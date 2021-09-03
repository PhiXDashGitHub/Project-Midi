using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RequestAnswer))]
public class SendSong : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;

    public int bpm;
    public string volume;
    public string reverb;

    public Object votingscene;
    int Maxamountoftrys;
    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        Maxamountoftrys = 3;
    }

    public void Send(string song)
    {
        StartCoroutine(SendInner(song));
    }

    public IEnumerator SendInner(string song)
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendSong.php", "PassWD=" + "1" + "&PlayerName=" + networkManager.Name + "&Song=" + song + "&LobbyId=" + networkManager.LobbyID + "&BPM=" + bpm + "&Ready=true" + "&Reverb=" + reverb + "&Volume=" + volume, this.gameObject);

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
            SceneManager.LoadScene(votingscene.name);
        }
        else
        {
            if (Maxamountoftrys < 3)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                StartCoroutine(SendInner(song));
                Maxamountoftrys++;
            }
            else
            {
                SceneManager.LoadScene(0);
            }
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
