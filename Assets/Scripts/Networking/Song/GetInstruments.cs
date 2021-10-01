using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RequestAnswer))]
public class GetInstruments : MonoBehaviour
{
    [Header("Networking")]
    RequestManager requestManager;
    NetworkManager networkManager;

    public void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
        StartCoroutine(Check());
    }

    public IEnumerator Check()
    {
        this.GetComponent<RequestAnswer>().Message = "";
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Lobby/GetAllLobbys.php", "PassWD=" + "1MRf!s13", this.gameObject);

        float time = 0;
        while (this.GetComponent<RequestAnswer>().Message.Length < 1)
        {
            if (time > networkManager.timeOut)
            {
                break;
            }
            time += Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }

        if (this.GetComponent<RequestAnswer>().Message.Length > 1)
        {
            string josn = "{\"Items\":" + this.GetComponent<RequestAnswer>().Message + "}";
            LobbyInfo[] LobbyInfo = JsonHelper.FromJson<LobbyInfo>(josn);
            for (int i = 0; i < LobbyInfo.Length; i++)
            {
                if (LobbyInfo[i].LobbyKey == networkManager.lobbyKey)
                {
                    this.GetComponent<NoteEditor>().StringToInstruments(LobbyInfo[i].Instruments);
                    break;
                }
            }
        }
        else
        {
            
        }
    }


    [System.Serializable]
    public class LobbyInfo
    {
        public int Id;
        public string Name;
        public string AmountofPlayer;
        public string LobbyKey;
        public string Instruments;
        public string Timestart;

        public static LobbyInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<LobbyInfo>(jsonString);
        }
    }
}
