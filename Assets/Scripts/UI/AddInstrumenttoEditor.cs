using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddInstrumenttoEditor : MonoBehaviour
{
    public List<string> Instruments = new List<string>();
    public NoteEditor Editor;



    public string InstrumentsToString()
    {
        string tmp = "[";

        for (int i = 0; i < Instruments.Count; i++)
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
