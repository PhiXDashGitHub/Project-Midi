using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Button))]
public class KeyBoardSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private AudioSource Tone;

    public KeyCode[] KeyBind;

    private Button Key;

    public bool Pressable;
    public bool FadeOut;

    public EventSystem Events;

    void Start()
    {
        Tone = GetComponent<AudioSource>();
        Key = GetComponent<Button>();
    }

    void Update()
    {
        Events = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        CheckKey();
        SetVolume();
        SetPitch();
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (Pressable == true)
        {
            Tone.loop = true;
        }

        Tone.Play();
    }

    public void OnPointerUp(PointerEventData data)
    {
        Tone.loop = false;

        if (FadeOut == false)
        {
            Tone.Stop();
        }

        Events.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

    void CheckKey()
    {
        for (int i = 0; i < KeyBind.Length; i++)
        {
            if (Input.GetKeyDown(KeyBind[i]))
            {
                Tone.Play();
            }

            if (Input.GetKey(KeyBind[i]))
            {
                if (Pressable == true)
                {
                    Tone.loop = true;
                }

                Key.Select();
            }
            else if (Input.GetKeyUp(KeyBind[i]))
            {
                Tone.loop = false;

                if (FadeOut == false)
                {
                    Tone.Stop();
                }

                Events.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            }
        }
    }

    void SetVolume()
    {
        Tone.volume = PlayerPrefs.GetFloat("KeyboardVolume");
    }

    void SetPitch()
    {
        Tone.pitch = PlayerPrefs.GetFloat("KeyboardPitch");
    }

    public void PlaySound()
    {
        Tone.Play();
    }
}
