using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PitchDial : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float fMinAngle;
    public float fMaxAngle;
    private float fAngle;

    public float fPitch;

    public float Sensitivity;

    bool InUse;

    void Start()
    {
        InUse = false;
    }

    void Update()
    {
        fAngle = fMinAngle + (fMaxAngle - fMinAngle) * fPitch / 3;

        transform.eulerAngles = new Vector3(0, 0, fAngle);

        if (InUse)
        {
            fPitch += Input.mouseScrollDelta.y * Sensitivity * Time.deltaTime;
            fPitch = Mathf.Clamp(fPitch, 0f, 3f);
        }

        PlayerPrefs.SetFloat("KeyboardPitch", fPitch);
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (Input.GetMouseButton(0))
        {
            InUse = true;
        }
        else if (Input.GetMouseButton(1))
        {
            InUse = false;
            fPitch = 1f;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        InUse = false;
    }
}
