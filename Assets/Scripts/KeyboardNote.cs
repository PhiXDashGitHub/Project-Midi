using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardNote : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    NoteEditor noteEditor;
    UIColors uiColors;
    Image image;

    public bool interactable;
    public int note;
    float startTime;
    string id;

    Color initColor;
    Color targetColor;
    bool pressed;

    void Start()
    {
        noteEditor = FindObjectOfType<NoteEditor>();
        uiColors = Resources.LoadAll<UIColors>("UI/")[0];
        image = GetComponent<Image>();

        initColor = image.color;
        targetColor = initColor;
    }

    void Update()
    {
        image.color = Color.Lerp(image.color, targetColor, 16 * Time.deltaTime);

        if (!interactable)
        {
            targetColor = uiColors.keyboardNoninteractableColor;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
            StopSound();
        }

        if (pressed)
        {
            targetColor = uiColors.keyboardSelectColor;
        }
        else if (targetColor != initColor)
        {
            targetColor = highlightedColor();
        }
    }

    public void SetInteractable()
    {
        interactable = true;
        targetColor = initColor;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (!interactable)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            pressed = true;

            PlaySound();
        }
        else
        {
            targetColor = highlightedColor();
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            return;
        }

        if (!interactable)
        {
            return;
        }

        pressed = true;
        targetColor = highlightedColor();

        PlaySound();
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!interactable)
        {
            return;
        }

        if (pressed)
        {
            SendData();
        }

        targetColor = initColor;
        pressed = false;

        StopSound();
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!interactable)
        {
            return;
        }

        if (pressed)
        {
            targetColor = highlightedColor();
        }

        pressed = false;

        StopSound();
        SendData();
    }

    void PlaySound()
    {
        startTime = NoteEditor.timer;

        if (interactable)
        {
            id = RandomString(10);
            noteEditor.PlayKeyboard(note, id);
        }
    }

    void StopSound()
    {
        noteEditor.StopKeyboardSound(id);
    }

    void SendData()
    {
        if (NoteEditor.timer - startTime > 0 && NoteEditor.recording)
        {
            NoteEditor.SendRecordData(note, startTime, NoteEditor.timer - startTime);
        }
    }

    public string RandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string output = "";

        for (int i = 0; i < length; i++)
        {
            output += chars[Random.Range(0, chars.Length)];
        }

        return output;
    }

    public Color highlightedColor()
    {
        return Color.Lerp(initColor, initColor == Color.black ? Color.white : Color.black, 0.2f);
    }
}
