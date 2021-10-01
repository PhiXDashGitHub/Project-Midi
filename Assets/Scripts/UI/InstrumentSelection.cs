using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelection : MonoBehaviour
{
    //Instrument Code: [Piano;...]
    public GameObject ListObjectPreset;
    public GameObject Content;
    public static int maxAmountOfInstruments = 6;
    public static int amountOfInstruments = 0;
    public static int minAmountOfInstruments = 1;

    Instrument[] instruments;


    public void Start()
    {
        amountOfInstruments = 0;

        StartCoroutine(LoadAll());
    }

    public IEnumerator LoadAll()
    {
        instruments = Resources.LoadAll<Instrument>("Instruments/");

        for (int i = 0; i < instruments.Length; i++)
        {
            GameObject go = Instantiate(ListObjectPreset, Content.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i * -100) - 50);
            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, i * 100);
            go.GetComponent<InstrumentButton>().name = instruments[i].name;
        }

        yield return null;
    }
}
