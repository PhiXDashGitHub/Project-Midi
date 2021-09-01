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
            yield return new WaitForSeconds(0.01f);
        }

        if (sender.TryGetComponent(out RequestAnswer answer) && go.TryGetComponent(out Getrequest request))
        {
            answer.Message = request.Message;
        }

        Destroy(go);
    }
}
