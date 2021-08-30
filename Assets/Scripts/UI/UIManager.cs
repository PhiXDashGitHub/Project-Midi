using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public Animation FadeIn;
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
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
}
