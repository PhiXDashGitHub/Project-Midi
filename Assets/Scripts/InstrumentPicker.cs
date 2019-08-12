using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstrumentPicker : MonoBehaviour
{
    public GameObject[] Instruments;

    public TextMeshProUGUI Label;

    void Start()
    {
        PlayerPrefs.SetString("CurrentInstrument", "Piano");
    }

    void Update()
    {
        for (int i = 0; i < Instruments.Length; i++)
        {
            if (Instruments[i].name == PlayerPrefs.GetString("CurrentInstrument"))
            {
                Instruments[i].SetActive(true);
            }
            else
            {
                Instruments[i].SetActive(false);
            }
        }
    }

    public void SetInstrument()
    {
        PlayerPrefs.SetString("CurrentInstrument", Label.text);
    }
}
