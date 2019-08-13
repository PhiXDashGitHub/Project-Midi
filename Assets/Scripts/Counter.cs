using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Counter : MonoBehaviour
{
    public float xStartPosition;

    public int BPM;

    RectTransform CounterTransform;

    public TextMeshProUGUI SpeedMeter;

    void Start()
    {
        BPM = 140;

        CounterTransform = GetComponent<RectTransform>();

        CounterTransform.localPosition = new Vector2(xStartPosition, transform.localPosition.y);
    }

    void FixedUpdate()
    {
        if (PlayerPrefs.GetString("TimerCount") == "True")
        {
            CounterTransform.Translate(Vector3.right * BPM * Time.fixedDeltaTime, Space.Self);
        }

        SpeedMeter.text = BPM + " BPM";
    }

    public void Reset()
    {
        CounterTransform.localPosition = new Vector2(xStartPosition, transform.localPosition.y);
    }

    public void IncreaseBPM(int Amount)
    {
        BPM += Amount;
    }
}
