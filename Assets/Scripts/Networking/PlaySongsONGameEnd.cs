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
    int tmpj = 0, tmpi = 0;

    public void PlaySong()
    {
        tmpplayers = GameObject.FindGameObjectsWithTag("Player");
        Playsongbool = true;
    }

    void Update()
    {
        if(Playsongbool == true)
        {
            currentsongtext.GetComponent<TextMeshProUGUI>().text = "Current Song: " + tmpplayers[tmpi].name;
            if (countdownisactiv == false && playersongplay == false)
            {
                if(tmpi == tmpplayers.Length)
                {
                    tmpi = 0;
                    Playsongbool = false;
                    return;
                }
                else
                {
                    currentsong = tmpplayers[tmpi].GetComponent<OnlinePlayerController>().PlayerSong;
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
            //tmptime = Int32.Parse(note);
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
