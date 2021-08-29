using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    Vector3 position;
    Vector3 localPosition;

    public Transform scrollRef;
    public float pos;

    void Start()
    {
        position = transform.position;
        localPosition = transform.localPosition;
    }

    void Update()
    {
        pos = NoteEditor.timer / NoteEditor.bpmOffset;

        Vector3 localPos = transform.position;
        localPos.x = scrollRef.TransformPoint(scrollRef.localPosition + scrollRef.transform.right * pos - scrollRef.transform.right * localPosition.x).x;
        localPos.y = position.y;
        transform.position = localPos;
    }
}
