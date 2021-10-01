using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Animation FadeIn;
    public Sprite[] AudioIcons;
    public AudioMixer audiomixer;
    public Image AudioButton;
    public TextMeshProUGUI[] playerNamePlaceholder;

    int audioCounter;

    public void Start()
    {
        audioCounter = 0;

        if (playerNamePlaceholder != null)
        {
            foreach (TextMeshProUGUI placeholder in playerNamePlaceholder)
            {
                placeholder.text = System.Environment.UserName;
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UpdateVolume();
        }
    }

    public void LoadSceneWithAnim(int i)
    {
        StartCoroutine(FadeAndLoad(true, i));
    }

    public void ActivateFade(GameObject g)
    {
        StartCoroutine(FadeAndLoad(true, -1, g, true));
    }

    public void DeActivateDelay(GameObject g)
    {
        StartCoroutine(FadeAndLoad(false, -1, g, false));
    }

    IEnumerator FadeAndLoad(bool playFade = true, int sceneIndex = -1, GameObject g = null, bool active = false)
    {
        if (playFade)
        {
            FadeIn.Play();
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (sceneIndex >= 0)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        if (g)
        {
            g.SetActive(active);
        }
    }

    public void SetAudio()
    {
        audioCounter++;

        if (audioCounter == AudioIcons.Length)
        {
            audioCounter = 0;
        }

        UpdateVolume();
    }

    void UpdateVolume()
    {
        AudioButton.sprite = AudioIcons[audioCounter];

        float Volume = -50 * ((float)audioCounter / 3);
        Volume = Volume == -50 ? -80 : Volume;
        audiomixer.SetFloat("Volume", Volume);
        PlayerPrefs.SetInt("Audiocounter", audioCounter);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
