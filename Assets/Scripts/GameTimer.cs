using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameTimer : MonoBehaviour
{
    TextMeshProUGUI Timer;

    public float MaxTimeSeconds;

    float TimeLeft;

    float Minutes;
    float Seconds;

    bool GameIsReady;

    void Start()
    {
        GameIsReady = true;

        Timer = GetComponent<TextMeshProUGUI>();

        TimeLeft = MaxTimeSeconds;
    }

    void Update()
    {
        if (GameIsReady)
        {
            Count();
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        Timer.text = Minutes.ToString("00") + ":" + Mathf.Round(Seconds).ToString("00");
    }

    void Count()
    {
        TimeLeft -= Time.deltaTime;
        Minutes = ((int)TimeLeft / 60);
        Seconds = (TimeLeft % 60);

        if (Mathf.Round(TimeLeft) == 0)
        {
            GameIsReady = false;
            EndGame();
        }
    }

    public void EndGame()
    {
        this.transform.root.GetComponent<OnlinePlayerController>().DisplayEndScreen();
        this.transform.root.GetComponent<PlaySongsONGameEnd>().PlaySong();
    }
}
