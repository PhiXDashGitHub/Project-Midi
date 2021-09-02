using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVisualizer : MonoBehaviour
{
    RectTransform limitRect;

    public AudioListener audioListener;
    public Vector2 minMaxHeight;
    public float animationSpeed = 4f;
    [Range(6, 13)] public int samples = 6;
    public float amplitude = 25f;

    void Start()
    {
        limitRect = GetComponent<RectTransform>();
    }

    public void Update()
    {
        float[] spectrum = new float[(int)Mathf.Pow(2, samples)];

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rectTransform = transform.GetChild(i).GetComponent<RectTransform>();

            float height = minMaxHeight.x + (spectrum[i] * (minMaxHeight.y - minMaxHeight.x)) * amplitude;
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, new Vector2(rectTransform.sizeDelta.x, Mathf.Clamp(height, minMaxHeight.x, limitRect.sizeDelta.y)), animationSpeed * Time.deltaTime);
        }
    }
}
