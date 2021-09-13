using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstrumentButton : MonoBehaviour
{
    public new string name;
    bool isselected;
    public Color SelectedColor;
    public Color DeSelectColor;
    CreateLobby createLobby;
    AddInstrumenttoEditor addInstrumenttoEditor;


    public void Start()
    {
        createLobby = FindObjectOfType<CreateLobby>();
        addInstrumenttoEditor = FindObjectOfType<AddInstrumenttoEditor>();
        isselected = false;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
    }

    public void SelectInstrument()
    {
        Debug.Log(addInstrumenttoEditor);
        Debug.Log(createLobby);

        if (isselected == false && InstrumentSelection.AmountOFInstruments < InstrumentSelection.MaxamountofInstruments)
        {
            isselected = true;
            if (createLobby)
            {
                FindObjectOfType<CreateLobby>().Instruments.Add(name + ";");
            }
            else if (addInstrumenttoEditor)
            {
                addInstrumenttoEditor.Instruments.Add(name + ";");
            }
            this.GetComponent<Image>().color = SelectedColor;
            InstrumentSelection.AmountOFInstruments++;
        }
        else if(isselected == true && InstrumentSelection.AmountOFInstruments > InstrumentSelection.minamountofInstruments)
        {
            isselected = false;
            if (createLobby)
            {
                createLobby.Instruments.Remove(name + ";");
            }
            else if (addInstrumenttoEditor)
            {
                addInstrumenttoEditor.Instruments.Remove(name + ";");
            }
            this.GetComponent<Image>().color = DeSelectColor;
            InstrumentSelection.AmountOFInstruments--;
        }
    }
}
