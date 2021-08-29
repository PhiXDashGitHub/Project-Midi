using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardNote : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int note;
    float startTime;

    public void OnPointerDown(PointerEventData data)
    {
        startTime = NoteEditor.timer;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (NoteEditor.timer - startTime > 0 && NoteEditor.recording)
        {
            NoteEditor.SendRecordData(note, startTime, NoteEditor.timer - startTime);
        }
    }
}
