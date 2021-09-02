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

        if (isselected == false && InstrumentSelection.AmountOFInstruments < InstrumentSelection.MaxamountofInstruments)
        {
            isselected = true;
            FindObjectOfType<CreateLobby>().Instruments.Add(name + ";");
            this.GetComponent<Image>().color = SelectedColor;
            InstrumentSelection.AmountOFInstruments++;
        }
        else if(isselected == true && InstrumentSelection.AmountOFInstruments > InstrumentSelection.minamountofInstruments)
        {
            isselected = false;
            FindObjectOfType<CreateLobby>().Instruments.Remove(name + ";");
            this.GetComponent<Image>().color = DeSelectColor;
            InstrumentSelection.AmountOFInstruments--;
        }
    }
}