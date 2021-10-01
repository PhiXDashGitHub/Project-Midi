using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddInstrumenttoEditor : MonoBehaviour
{
    public string[] Instruments;
    public NoteEditor Editor;

    void Start()
    {
        Instruments = new string[InstrumentSelection.maxAmountOfInstruments];

        for (int i = 0; i < Instruments.Length; i++)
        {
            Instruments[i] = "";
        }
    }

    public string InstrumentsToString()
    {
        string tmp = "[";

        for (int i = 0; i < Instruments.Length; i++)
        {
            tmp += Instruments[i];
        }

        tmp += "]";
        return tmp;
    }

    public void Save()
    {
        Editor.StringToInstruments(InstrumentsToString());
    }
}
