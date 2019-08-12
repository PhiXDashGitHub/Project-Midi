using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{

    public void Fadeinout(GameObject FadeIn)
    {
        StartCoroutine(FadeIEmnum(FadeIn));
    }

    public IEnumerator FadeIEmnum(GameObject FadeIn)
    {
        this.GetComponent<Animator>().SetBool("Fade", true);
        yield return new WaitForSeconds(1);
        FadeIn.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<Animator>().SetBool("Fade", false);
    }
}
