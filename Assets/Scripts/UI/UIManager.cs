using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;


public class UIManager : MonoBehaviour
{
    public Animation FadeIn;
    public Sprite[] AudioIcons;
    int audiocounter;
    public AudioMixer audiomixer;
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
        yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(0.5f);
        g.SetActive(true);
    }
    public IEnumerator DeActivateInner(GameObject g)
    {
        yield return new WaitForSeconds(0.5f);
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
        Debug.Log(audiocounter);
        Debug.Log(-80 * ((float)audiocounter / 3));
        audiomixer.SetFloat("Volume",-80 * ((float)audiocounter /3));
    }


    public void OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }
}
