using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    //Prefab for Getrquest Object
    public GameObject Getrequest;
    

    public void Post(string URL,string data,GameObject sender)
    {
        GameObject go = Instantiate(Getrequest,this.transform);
        go.GetComponent<Getrequest>().Post(URL,data);
        StartCoroutine(Wait(go,sender));
    }

    public IEnumerator Wait(GameObject go, GameObject sender)
    {
        while (go.GetComponent<Getrequest>().Message == null)
        {
            yield return new WaitForSecondsRealtime(0.01f);
        }

        try
        {
            sender.GetComponent<RequestAnswer>().Message = go.GetComponent<Getrequest>().Message;

            Destroy(go);
        }
        catch (System.Exception) { }
    }
}
