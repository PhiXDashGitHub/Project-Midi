using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePlaySound : MonoBehaviour
{
    GameObject KeyboardParent;
    public GameObject SelectedKeyboard;

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
    }
}
