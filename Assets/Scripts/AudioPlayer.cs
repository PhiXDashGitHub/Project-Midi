using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    float dur;
    [HideInInspector] public Coroutine coroutine;

    public void PlayForSeconds(float startTime, float duration, bool useRealTime = false, float decay = 4)
    {
        dur = duration;

        coroutine = StartCoroutine(IPlayForSeconds(startTime, useRealTime, decay));
        StartCoroutine(PlayDelayed(startTime - (useRealTime ? Time.time : NoteEditor.timer)));
    }

    public void StopPlayback()
    {
        StopCoroutine(coroutine);
        dur = 0.1f;
        coroutine = StartCoroutine(IPlayForSeconds(Time.time, true));
    }

    IEnumerator PlayDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        GetComponent<AudioSource>().Play();
    }

    IEnumerator IPlayForSeconds(float startTime, bool useRealTime = false, float decay = 4)
    {
        if ((useRealTime ? Time.time : NoteEditor.timer) < startTime + dur)
        {
            yield return new WaitForSecondsRealtime((startTime + dur) - (useRealTime ? Time.time : NoteEditor.timer));
            //Debug.Log("EndTimer: " + NoteEditor.timer);
            coroutine = StartCoroutine(IPlayForSeconds(startTime, useRealTime, decay));
        }
        else if ((useRealTime ? Time.time : NoteEditor.timer) > startTime + dur && GetComponent<AudioSource>().volume > 0)
        {
            GetComponent<AudioSource>().volume -= decay * Time.deltaTime;

            yield return new WaitForSecondsRealtime(Time.deltaTime);
            coroutine = StartCoroutine(IPlayForSeconds(startTime, useRealTime, decay));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
