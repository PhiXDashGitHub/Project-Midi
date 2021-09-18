using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    [Range(0, 1)] public float scrollValue;
    public Vector2 scrollRangeX;

    public Slider scrollBar;

    void Update()
    {
        scrollValue = scrollBar.value;
        transform.localPosition = new Vector2(Mathf.Lerp(scrollRangeX.x, scrollRangeX.y, scrollValue), transform.localPosition.y);
    }

    public void SetScrollValue(float value)
    {
        value = Mathf.Clamp01(value);

        scrollBar.value = value;
        scrollValue = value;
    }
}
