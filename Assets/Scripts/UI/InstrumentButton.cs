using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstrumentButton : MonoBehaviour
{
    public new string name;
    bool isselected;
    public Color DeSelectColor;
    UIColors uiColors;
    CreateLobby createLobby;
    AddInstrumenttoEditor addInstrumenttoEditor;


    public void Start()
    {
        createLobby = FindObjectOfType<CreateLobby>();
        addInstrumenttoEditor = FindObjectOfType<AddInstrumenttoEditor>();
        isselected = false;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;

        uiColors = Resources.LoadAll<UIColors>("UI/")[0];
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
                CreateLobby lobby = FindObjectOfType<CreateLobby>();

                for (int i = 0; i < InstrumentSelection.MaxamountofInstruments; i++)
                {
                    if (lobby.Instruments[i] == "" || lobby.Instruments[i] == null)
                    {
                        GetComponent<Image>().color = uiColors.noteColors[i];
                        lobby.Instruments[i] = name + ";";
                        break;
                    }
                }
            }
            else if (addInstrumenttoEditor)
            {
                for (int i = 0; i < InstrumentSelection.MaxamountofInstruments; i++)
                {
                    if (addInstrumenttoEditor.Instruments[i] == "" || addInstrumenttoEditor.Instruments[i] == null)
                    {
                        GetComponent<Image>().color = uiColors.noteColors[i];
                        addInstrumenttoEditor.Instruments[i] = name + ";";
                        break;
                    }
                }
            }

            InstrumentSelection.AmountOFInstruments++;
        }
        else if(isselected == true && InstrumentSelection.AmountOFInstruments > InstrumentSelection.minamountofInstruments)
        {
            isselected = false;
            if (createLobby)
            {
                for (int i = 0; i < InstrumentSelection.MaxamountofInstruments; i++)
                {
                    if (createLobby.Instruments[i] == name + ";")
                    {
                        createLobby.Instruments[i] = "";
                        break;
                    }
                }
            }
            else if (addInstrumenttoEditor)
            {
                for (int i = 0; i < InstrumentSelection.MaxamountofInstruments; i++)
                {
                    if (addInstrumenttoEditor.Instruments[i] == name + ";")
                    {
                        addInstrumenttoEditor.Instruments[i] = "";
                        break;
                    }
                }
            }

            GetComponent<Image>().color = DeSelectColor;
            InstrumentSelection.AmountOFInstruments--;
        }
    }
}
