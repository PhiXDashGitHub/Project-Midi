using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RequestAnswer))]
public class SendSong : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;
    RequestAnswer requestAnswer;

    public int bpm;
    public string volume;
    public string reverb;

    public int votingSceneIndex = 2;
    int maxAmountOfTrys;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        requestAnswer = GetComponent<RequestAnswer>();

        maxAmountOfTrys = 3;
    }

    //Sends the Song
    public void Send(string song)
    {
        StartCoroutine(SendInner(song));
    }

    //Sends the Song via Post Request
    public IEnumerator SendInner(string song)
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendSong.php", "PassWD=" + "1" + "&PlayerName=" + networkManager.name + "&Song=" + song + "&LobbyId=" + networkManager.lobbyID + "&BPM=" + bpm + "&Ready=true" + "&Reverb=" + reverb + "&Volume=" + volume, gameObject);

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
            SceneManager.LoadScene(votingSceneIndex);
        }
        else
        {
            //Try again 3 times or else return to main Menu
            if (maxAmountOfTrys < 3)
            {
                yield return new WaitForSecondsRealtime(Time.deltaTime);

                StartCoroutine(SendInner(song));
                maxAmountOfTrys++;
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
