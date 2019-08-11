using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{

    public void Fadeinout(GameObject fadein)
    {
        StartCoroutine(FadeIEmnum(fadein));
    }

    public IEnumerator FadeIEmnum(GameObject fadein)
    {
        this.GetComponent<Animator>().SetBool("Fade", true);
        yield return new WaitForSeconds(1);
        fadein.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<Animator>().SetBool("Fade", false);
    }
}
