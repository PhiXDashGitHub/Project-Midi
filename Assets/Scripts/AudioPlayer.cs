using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public void PlayForSeconds(float startTime, float duration, bool useRealTime = false, float decay = 4)
    {
        StartCoroutine(IPlayForSeconds(startTime, duration, useRealTime, decay));
        StartCoroutine(PlayDelayed(startTime - (useRealTime ? Time.time : NoteEditor.timer)));
    }

    IEnumerator PlayDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<AudioSource>().Play();
    }

    IEnumerator IPlayForSeconds(float startTime, float duration, bool useRealTime = false, float decay = 4)
    {
        if ((useRealTime ? Time.time : NoteEditor.timer) < startTime + duration)
        {
            yield return new WaitForSeconds((startTime + duration) - (useRealTime ? Time.time : NoteEditor.timer));
            //Debug.Log("EndTimer: " + NoteEditor.timer);
            StartCoroutine(IPlayForSeconds(startTime, duration, useRealTime, decay));
        }
        else if ((useRealTime ? Time.time : NoteEditor.timer) > startTime + duration && GetComponent<AudioSource>().volume > 0)
        {
            GetComponent<AudioSource>().volume -= decay * Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
            StartCoroutine(IPlayForSeconds(startTime, duration, useRealTime, decay));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
