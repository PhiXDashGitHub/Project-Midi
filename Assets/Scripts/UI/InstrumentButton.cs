using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstrumentButton : MonoBehaviour
{
    public string name;
    bool isselected;
    public Color SelectedColor;
    public Color DeSelectColor;
    

    public void Start()
    {
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
    }

    public void SelectInstrument()
    {
        if (isselected == false && FindObjectOfType<InstrumentSelection>().AmountOFInstruments < FindObjectOfType<InstrumentSelection>().MaxamountofInstruments)
        {
            isselected = true;
            FindObjectOfType<CreateLobby>().Instruments.Add(name + ";");
            this.GetComponent<Image>().color = SelectedColor;
            FindObjectOfType<InstrumentSelection>().AmountOFInstruments++;
        }
        else
        {
            isselected = false;
            FindObjectOfType<CreateLobby>().Instruments.Add(name + ";");
            this.GetComponent<Image>().color = DeSelectColor;
            FindObjectOfType<InstrumentSelection>().AmountOFInstruments--;
        }
    }
}
