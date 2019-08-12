using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterBar : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D Note)
    {
        if (Note.tag == "Note")
        {
            Note.GetComponent<NotePlaySound>().PlaySound();
            Note.GetComponent<Image>().color = Color.green;
        }
    }

    public void OnTriggerExit2D(Collider2D Note)
    {
        if (Note.tag == "Note")
        {
            Note.GetComponent<Image>().color = Color.white;
        }
    }
}
