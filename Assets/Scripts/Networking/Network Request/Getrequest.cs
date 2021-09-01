using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class Getrequest : MonoBehaviour
{
    public string Message;

    public void Start()
    {
        Message = null;
    }

    public void Get(string URL)
    {
        Message = null;
        StartCoroutine(GetRequest(URL));
    }
    public void Post(string URL, string data)
    {
        Message = null;
        StartCoroutine(PostRequest(URL, data));
    }
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                try
                {
                    //Message = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data, 3, webRequest.downloadHandler.data.Length - 3);
                    Message = webRequest.downloadHandler.text;
                }
                catch (Exception)
                {
                    Message = webRequest.downloadHandler.text;
                }
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
    IEnumerator PostRequest(string URL, string data)
    {
        //field1=foo&field2=bar
        //Data has to be in above format

        WWWForm form = new WWWForm();
        string[] split = data.Split('&');

        for (int i = 0; i < split.Length; i++)
        {
            form.AddField(split[i].Split('=')[0], split[i].Split('=')[1]);
        }

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.Log(www.error + " : " + URL);
            }
            else
            {
                Message = www.downloadHandler.text;
            }
            Debug.Log(":\nReceived: " + www.downloadHandler.text +" : " + URL);
        }

    }

}