using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fade : MonoBehaviour
{
    public void Start()
    {
        GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<Canvas>().enabled = false;
        GameObject.FindGameObjectWithTag("LobbyManager").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
    }


    public void Fadeinout(GameObject FadeIn)
    {
        StartCoroutine(FadeIEmnum(FadeIn));
    }

    public void FadeLobbyinout()
    {
        StartCoroutine(FadeILobbyEmnum(GameObject.FindGameObjectWithTag("LobbyManager")));
    }

    public IEnumerator FadeIEmnum(GameObject FadeIn)
    {
        this.GetComponent<Animator>().SetBool("Fade", true);
        yield return new WaitForSeconds(1);
        FadeIn.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<Animator>().SetBool("Fade", false);
    }
    public IEnumerator FadeILobbyEmnum(GameObject FadeIn)
    {
        this.GetComponent<Animator>().SetBool("Fade", true);
        yield return new WaitForSeconds(1);
        FadeIn.GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<Animator>().SetBool("Fade", false);
    }
}
