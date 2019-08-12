using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public float xStartPosition;

    public float BPM;

    RectTransform CounterTransform;

    void Start()
    {
        CounterTransform = GetComponent<RectTransform>();

        CounterTransform.localPosition = new Vector2(xStartPosition, transform.localPosition.y);
    }

    void Update()
    {
        if (PlayerPrefs.GetString("TimerCount") == "True")
        {
            CounterTransform.Translate(Vector3.right * BPM * Time.deltaTime, Space.Self);
        }
    }

    public void Reset()
    {
        CounterTransform.localPosition = new Vector2(xStartPosition, transform.localPosition.y);
    }
}
