using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NotePlaySound : MonoBehaviour
{
    GameObject KeyboardParent;
    public GameObject SelectedKeyboard;
    public static float timeoflastnote = 0;
    public static string[] Song = new string[1];
    GameObject Key;

    void Update()
    {
        KeyboardParent = GameObject.Find("KeyboardRig");

        for (int k = 0; k < KeyboardParent.transform.childCount; k++)
        {
            if (KeyboardParent.transform.GetChild(k).name == PlayerPrefs.GetString("CurrentInstrument"))
            {
                SelectedKeyboard = KeyboardParent.transform.GetChild(k).gameObject;
            }
        }
    }

    public void OnTriggerStay2D(Collider2D Line)
    {
        if (Line.tag == "Line")
        {
            for (int i = 0; i < SelectedKeyboard.transform.childCount; i++)
            {
                if (SelectedKeyboard.transform.GetChild(i).name == Line.name)
                {
                    Key = SelectedKeyboard.transform.GetChild(i).gameObject;
                }
            }
            this.gameObject.name = Line.name;
            transform.position = new Vector2(transform.position.x, Line.transform.position.y);
        }
        else if (Line.tag == "Space")
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound()
    {
        Key.GetComponent<KeyBoardSound>().PlaySound();
        Save(this.gameObject.name);
    }

    public void Save(string name)
    {
        string[] tmpstring = new string[NotePlaySound.Song.Length + 2];
        for (int i = 0; i < NotePlaySound.Song.Length; i++)
        {
            tmpstring[i] = NotePlaySound.Song[i];
        }
        tmpstring[NotePlaySound.Song.Length] = name;
        tmpstring[NotePlaySound.Song.Length + 1] = (EditorTiming.fElapsedTime - NotePlaySound.timeoflastnote).ToString();
        NotePlaySound.Song = tmpstring;

        GameObject[] tmpplayers = GameObject.FindGameObjectsWithTag("Player");

        NotePlaySound.timeoflastnote = EditorTiming.fElapsedTime;
        Debug.Log("time of last note" + NotePlaySound.timeoflastnote);
        for (int i = 0; i < tmpplayers.Length; i++)
        {
            if(this.transform.root.gameObject.name == tmpplayers[i].name)
            {
                tmpplayers[i].GetComponent<OnlinePlayerController>().PlayerSong = NotePlaySound.Song;
            }
        }
    }
}
