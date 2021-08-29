using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorKeyboard : MonoBehaviour
{
    Vector3 position;

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = position.x;
        transform.position = pos;
    }
}
