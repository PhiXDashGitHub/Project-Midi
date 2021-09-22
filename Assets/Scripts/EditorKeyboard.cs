using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorKeyboard : MonoBehaviour
{
    public bool fixY = false;
    Vector3 position;

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = !fixY ? position.x : pos.x;
        pos.y = fixY ? position.y : pos.y;
        transform.position = pos;
    }
}
