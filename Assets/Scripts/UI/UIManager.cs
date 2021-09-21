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
    int audiocounter;
    public AudioMixer audiomixer;
    public Image AudioButton;
    public TextMeshProUGUI[] playerNamePlaceholder;

    public void Start()
    {
        audiocounter = 0;

        if (playerNamePlaceholder != null)
        {
            foreach (TextMeshProUGUI placeholder in playerNamePlaceholder)
            {
                placeholder.text = System.Environment.UserName;
            }
        }

        if (PlayerPrefs.GetInt("Audiocounter") != audiocounter && SceneManager.GetActiveScene().buildIndex == 0)
        {
            audiocounter = PlayerPrefs.GetInt("Audiocounter");
            AudioButton.sprite = AudioIcons[audiocounter];
            float Volume = -50 * ((float)audiocounter / 3);
            Volume = Volume == -50 ? -80 : Volume;
            audiomixer.SetFloat("Volume", Volume);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadSceneWithAnim(int i)
    {
        StartCoroutine(LoadInner(i));
    }

    public IEnumerator LoadInner(int i)
    {
        FadeIn.Play();
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(i);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ActivateFade(GameObject g)
    {
        StartCoroutine(FadeInner(g));
    }

    public void DeActivateDelay(GameObject g)
    {
        StartCoroutine(DeActivateInner(g));
    }

    public IEnumerator FadeInner(GameObject g)
    {
        FadeIn.Play();
        yield return new WaitForSecondsRealtime(0.5f);
        g.SetActive(true);
    }

    public IEnumerator DeActivateInner(GameObject g)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        g.SetActive(false);
    }

    public void SetAudio(Image Audiobutton)
    {
        audiocounter++;
        if (audiocounter == AudioIcons.Length)
        {
            audiocounter = 0;
        }
        Audiobutton.sprite = AudioIcons[audiocounter];
        float Volume = -50 * ((float)audiocounter / 3);
        Volume = Volume == -50 ? -80 : Volume;
        audiomixer.SetFloat("Volume", Volume);
        PlayerPrefs.SetInt("Audiocounter", audiocounter);
    }

    public void OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }
}
