using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class Request : MonoBehaviour
{
    public string Message;

    public void Start()
    {
        Message = null;
    }

    //Starts GetRequest
    public void Get(string URL)
    {
        Message = null;
        StartCoroutine(GetRequest(URL));
    }

    //Starts PostRequest
    public void Post(string URL, string data)
    {
        Message = null;
        StartCoroutine(PostRequest(URL, data));
    }

    //GetRequest using UnityWeb
    IEnumerator GetRequest(string url)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        yield return webRequest.SendWebRequest();

        string[] pages = url.Split('/');
        int page = pages.Length - 1;

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(pages[page] + ": Error: " + webRequest.error);
        }
        else
        {
            try
            {
                Message = webRequest.downloadHandler.text;
            }
            catch (Exception)
            {
                Message = webRequest.downloadHandler.text;
            }

            Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
        }
    }

    //PostRequest using UnityWeb
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

        using UnityWebRequest www = UnityWebRequest.Post(URL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.LogError(www.error + " : " + URL);
        }
        else
        {
            Message = www.downloadHandler.text;
        }

        Debug.Log(":\nReceived: " + www.downloadHandler.text + " : " + URL);
    }
}