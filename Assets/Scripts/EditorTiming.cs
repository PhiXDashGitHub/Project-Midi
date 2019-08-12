using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditorTiming : MonoBehaviour
{
    public float fElapsedTime;

    public TextMeshProUGUI Timer;

    public bool TimerCount;
    public bool IsPaused;

    void Start()
    {
        fElapsedTime = 0f;
        TimerCount = false;
        IsPaused = false;
    }

    void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        PlayerPrefs.SetString("TimerCount", TimerCount.ToString());

        if (TimerCount == true)
        {
            fElapsedTime += Time.deltaTime;
        }

        string Minutes = ((int)fElapsedTime / 60).ToString();
        string Seconds = (fElapsedTime % 60).ToString("f2");
        Timer.text = Minutes + ":" + Seconds;
    }

    public void StartTimer()
    {
        TimerCount = true;
    }

    public void PauseTimer()
    {
        IsPaused = true;
        TimerCount = false;
    }

    public void StopTimer()
    {
        TimerCount = false;
        IsPaused = false;
        fElapsedTime = 0f;
    }
}
