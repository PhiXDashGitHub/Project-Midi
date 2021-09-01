using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RequestAnswer))]
public class SendRating : MonoBehaviour
{
    RequestManager requestManager;
    NetworkManager networkManager;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        requestManager = FindObjectOfType<RequestManager>();
    }

    public void Send()
    {
        StartCoroutine(SendInner());
    }

    public IEnumerator SendInner()
    {
        requestManager.Post("https://www.linuslepschies.de/ProjectMidi/Game/SendReady.php", "PassWD=" + "1" + "&PlayerName=" + networkManager.Name + "&LobbyId=" + networkManager.LobbyID + "&VotingReady=true", this.gameObject);

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
            Debug.Log("Voting Ready Updated");
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
