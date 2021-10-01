using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    //Prefab for GetRequest Object
    public GameObject getRequestPrefab;
    
    //Post Request
    public void Post(string URL, string data, GameObject sender)
    {
        GameObject go = Instantiate(getRequestPrefab, transform);
        go.GetComponent<Request>().Post(URL,data);
        StartCoroutine(Wait(go,sender));
    }

    //Wait for Message
    public IEnumerator Wait(GameObject go, GameObject sender)
    {
        while (go.GetComponent<Request>().Message == null)
        {
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }

        try
        {
            sender.GetComponent<RequestAnswer>().Message = go.GetComponent<Request>().Message;

            Destroy(go);
        }
        catch (System.Exception) { }
    }
}
