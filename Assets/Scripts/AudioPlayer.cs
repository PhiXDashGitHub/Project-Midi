using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public void PlayForSeconds(float startTime, float duration, bool useRealTime = false)
    {
        GetComponent<AudioSource>().PlayScheduled(AudioSettings.dspTime + (startTime - (useRealTime ? Time.time : NoteEditor.timer)));
        StartCoroutine(IPlayForSeconds(startTime, duration, useRealTime));
    }

    IEnumerator IPlayForSeconds(float startTime, float duration, bool useRealTime = false)
    {
        if ((useRealTime ? Time.time : NoteEditor.timer) < startTime + duration)
        {
            yield return new WaitForSeconds((startTime + duration) - (useRealTime ? Time.time : NoteEditor.timer));
            //Debug.Log("EndTimer: " + NoteEditor.timer);
            StartCoroutine(IPlayForSeconds(startTime, duration, useRealTime));
        }
        else if ((useRealTime ? Time.time : NoteEditor.timer) > startTime + duration && GetComponent<AudioSource>().volume > 0)
        {
            GetComponent<AudioSource>().volume -= 8 * Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
            StartCoroutine(IPlayForSeconds(startTime, duration, useRealTime));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
