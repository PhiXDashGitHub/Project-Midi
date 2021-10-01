using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RequestAnswer))]
public class GetInstruments : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;
    RequestAnswer requestAnswer;

    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        requestAnswer = GetComponent<RequestAnswer>();

        StartCoroutine(Check());
    }

    //Checks for Instruments of Lobbys and sets them
    public IEnumerator Check()
    {
        requestAnswer.Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php", "PassWD=" + "1MRf!s13", gameObject);

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
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(json);

            for (int i = 0; i < LobbyInfo.Length; i++)
            {
                //If lobby found, get its Instruments
                if (LobbyInfo[i].LobbyKey == networkManager.lobbyKey)
                {
                    GetComponent<NoteEditor>().StringToInstruments(LobbyInfo[i].Instruments);
                    break;
                }
            }
        }
    }
}
