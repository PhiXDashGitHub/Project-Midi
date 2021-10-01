using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstrumentButton : MonoBehaviour
{
    public new string name;
    public Color DeSelectColor;

    bool isSelected;
    bool lobby;
    UIColors uiColors;
    CreateLobby createLobby;
    AddInstrumenttoEditor addInstrumentToEditor;


    public void Start()
    {
        isSelected = false;

        createLobby = FindObjectOfType<CreateLobby>();
        lobby = createLobby;
        addInstrumentToEditor = FindObjectOfType<AddInstrumenttoEditor>();

        GetComponentInChildren<TextMeshProUGUI>().text = name;
        uiColors = Resources.LoadAll<UIColors>("UI/")[0];
    }

    public void SelectInstrument()
    {
        if (!isSelected && InstrumentSelection.amountOfInstruments < InstrumentSelection.maxAmountOfInstruments)
        {
            SetInstruments(true);
        }
        else if (isSelected && InstrumentSelection.amountOfInstruments > InstrumentSelection.minAmountOfInstruments)
        {
            SetInstruments(false);
        }
    }

    void SetInstruments(bool select)
    {
        isSelected = select;

        for (int i = 0; i < InstrumentSelection.maxAmountOfInstruments; i++)
        {
            if (lobby)
            {
                if (createLobby.Instruments[i] == (select ? "" : name + ";"))
                {
                    if (select)
                    {
                        GetComponent<Image>().color = uiColors.noteColors[i];
                    }

                    createLobby.Instruments[i] = select ? name + ";" : "";
                    break;
                }
            }
            else
            {
                if (addInstrumentToEditor.Instruments[i] == (select ? "" : name + ";"))
                {
                    if (select)
                    {
                        GetComponent<Image>().color = uiColors.noteColors[i];
                    }

                    addInstrumentToEditor.Instruments[i] = select ? name + ";" : "";
                    break;
                }
            }
        }

        if (!select)
        {
            GetComponent<Image>().color = DeSelectColor;
            InstrumentSelection.amountOfInstruments--;
        }
        else
        {
            InstrumentSelection.amountOfInstruments++;
        }
    }
}
