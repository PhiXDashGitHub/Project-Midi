using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardNote : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    NoteEditor noteEditor;
    Button button;

    public int note;
    float startTime;

    void Start()
    {
        noteEditor = FindObjectOfType<NoteEditor>();
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        startTime = NoteEditor.timer;

        if (button.interactable)
        {
            noteEditor.PlayKeyboardSound(note);
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (NoteEditor.timer - startTime > 0 && NoteEditor.recording)
        {
            NoteEditor.SendRecordData(note, startTime, NoteEditor.timer - startTime);
        }
    }
}
