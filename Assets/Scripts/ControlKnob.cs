using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlKnob : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Range(0, 1)] public float value = 0.25f;

    public Vector2 angleLimits;
    public float sensitivity;

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

            value = dragValue + (Input.mousePosition - dragStart).x / sensitivity;
            value = Mathf.Clamp01(value);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        mouseOver = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!dragging)
        {
            mouseOver = false;
        }
    }
}
