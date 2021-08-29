using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlKnob : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Range(0, 1)] public float value = 0.25f;

    public Vector2 angleLimits;

    bool mouseOver;
    bool dragging;

    Vector3 dragStart;
    float dragValue;

    void Update()
    {
        Vector3 angle = transform.eulerAngles;
        angle.z = Mathf.Lerp(angleLimits.x, angleLimits.y, value);
        transform.eulerAngles = angle;

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = Input.mousePosition;
            dragValue = value;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseOver = false;
            dragging = false;
        }

        if (mouseOver && Input.GetMouseButton(0))
        {
            dragging = true;

            value = dragValue + (Input.mousePosition - dragStart).x / 50f;
            value = Mathf.Clamp01(value);
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!dragging)
        {
            mouseOver = false;
        }
    }
}
