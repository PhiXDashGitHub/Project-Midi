using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelection : MonoBehaviour
{
    //Instrument Code: [Piano;...]
    public GameObject ListObjectPreset;
    public GameObject Content;
    public int MaxamountofInstruments;
    public int AmountOFInstruments = 0;

    public void Start()
    {
       Instrument[] arry = Resources.LoadAll<Instrument>("Instruments/");
        for (int i = 0; i < arry.Length; i++)
        {
            GameObject go = Instantiate(ListObjectPreset, Content.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i * -100) - 50);
            go.GetComponent<InstrumentButton>().name = arry[i].name;
        }
    }
}
