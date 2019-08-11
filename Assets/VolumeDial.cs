using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VolumeDial : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float fMinAngle;
    public float fMaxAngle;
    private float fAngle;

    public float fVolume;

    public float Sensitivity;

    bool InUse;

    void Start()
    {
        InUse = false;
    }

    void Update()
    {
        fAngle = fMinAngle + (fMaxAngle - fMinAngle) * fVolume;

        transform.eulerAngles = new Vector3(0, 0, fAngle);

        if (InUse)
        {
            fVolume += Input.mouseScrollDelta.y * Sensitivity * Time.deltaTime;
            fVolume = Mathf.Clamp01(fVolume);
        }

        PlayerPrefs.SetFloat("KeyboardVolume", fVolume);
    }

    public void OnPointerDown(PointerEventData data)
    {
        InUse = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        InUse = false;
    }
}
