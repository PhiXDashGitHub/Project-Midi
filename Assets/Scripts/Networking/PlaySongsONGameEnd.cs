using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PlaySongsONGameEnd : MonoBehaviour
{
    GameObject[] tmpplayers;
    public GameObject keyparent;
    public GameObject currentsongtext;
    string[] currentsong;
    bool songisplayed = false, countdownisactiv = false, Playsongbool = false, playersongplay = false;
    int tmpj, tmpi;

    public void PlaySong()
    {
        tmpplayers = GameObject.FindGameObjectsWithTag("Player");
        Playsongbool = true;
    }

    public void Start()
    {
        tmpi = 0;
        tmpj = 0;
    }

    void Update()
    {
        if(Playsongbool == true)
        {
            if (countdownisactiv == false && playersongplay == false)
            {
                if(tmpi >= tmpplayers.Length)
                {
                    tmpi = 0;
                    Playsongbool = false;
                    Debug.Log("Song has stoppt playing");
                    currentsongtext.GetComponent<TextMeshProUGUI>().text = "No Song is Playing!";
                    return;
                }
                else
                {
                    currentsong = tmpplayers[tmpi].GetComponent<OnlinePlayerController>().PlayerSong;
                    try
                    {
                        currentsongtext.GetComponent<TextMeshProUGUI>().text = "Current Song: " + tmpplayers[tmpi].name;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        currentsongtext.GetComponent<TextMeshProUGUI>().text = "No Song is Playing!";
                    }
                    StartCoroutine(Countdown());
                    tmpi++;
                }
            }
            if(songisplayed == false && playersongplay == true)
            {
                if (tmpj == currentsong.Length)
                {
                    tmpj = 0;
                    playersongplay = false;
                }
                else
                {
                    StartCoroutine(Playsong(currentsong[tmpj]));
                    tmpj++;
                }
            }
        }
    }


    public IEnumerator Countdown()
    {
        countdownisactiv = true;
        yield return new WaitForSeconds(5);
        countdownisactiv = false;
        playersongplay = true;
    }

    public IEnumerator Playsong(string note)
    {
        float tmptime = 0;
        songisplayed = true;
        try
        {
            tmptime = float.Parse(note);
            Debug.Log("Time Play works " + tmptime);
        }
        catch (FormatException)
        {
            for (int i = 0; i < keyparent.transform.childCount; i++)
            {
                if (keyparent.transform.GetChild(i).name == note)
                {
                    keyparent.transform.GetChild(i).GetComponent<KeyBoardSound>().PlaySound();
                }
            }
        }
        catch (OverflowException)
        {
            for (int i = 0; i < keyparent.transform.childCount; i++)
            {
                if (keyparent.transform.GetChild(i).name == note)
                {
                    keyparent.transform.GetChild(i).GetComponent<KeyBoardSound>().PlaySound();
                }
            }
        }
        yield return new WaitForSeconds(tmptime);
        songisplayed = false;
    }
}
