using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelection : MonoBehaviour
{
    //Instrument Code: [Piano;...]
    public GameObject ListObjectPreset;
    public GameObject Content;
    public static int MaxamountofInstruments;
    public static int AmountOFInstruments;
    public static int minamountofInstruments;

    Instrument[] arry;

    public void Awake()
    {
        arry = Resources.LoadAll<Instrument>("Instruments/");
    }


    public void Start()
    {
        MaxamountofInstruments = 6;
        minamountofInstruments = 1;

        for (int i = 0; i < arry.Length; i++)
        {
            GameObject go = Instantiate(ListObjectPreset, Content.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i * -100) - 50);
            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,i*100);
            go.GetComponent<InstrumentButton>().name = arry[i].name;
        }
    }
}
