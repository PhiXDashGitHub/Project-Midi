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
        Debug.Log(FindObjectOfType<AddInstrumenttoEditor>());
        Debug.Log(FindObjectOfType<CreateLobby>());

        if (isselected == false && InstrumentSelection.AmountOFInstruments < InstrumentSelection.MaxamountofInstruments)
        {
            Debug.Log("Is selected");
            isselected = true;
            if (FindObjectOfType<CreateLobby>())
            {
                FindObjectOfType<CreateLobby>().Instruments.Add(name + ";");
            }
            else if (FindObjectOfType<AddInstrumenttoEditor>())
            {
                FindObjectOfType<AddInstrumenttoEditor>().Instruments.Add(name + ";");
            }
            this.GetComponent<Image>().color = SelectedColor;
            InstrumentSelection.AmountOFInstruments++;
        }
        else if(isselected == true && InstrumentSelection.AmountOFInstruments > InstrumentSelection.minamountofInstruments)
        {
            isselected = false;
            if (FindObjectOfType<CreateLobby>())
            {
                FindObjectOfType<CreateLobby>().Instruments.Remove(name + ";");
            }
            else if (FindObjectOfType<AddInstrumenttoEditor>())
            {
                FindObjectOfType<AddInstrumenttoEditor>().Instruments.Remove(name + ";");
            }
            this.GetComponent<Image>().color = DeSelectColor;
            InstrumentSelection.AmountOFInstruments--;
        }
    }
}
