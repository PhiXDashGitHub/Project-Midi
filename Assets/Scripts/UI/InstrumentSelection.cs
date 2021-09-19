using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelection : MonoBehaviour
{
    //Instrument Code: [Piano;...]
    public GameObject ListObjectPreset;
    public GameObject Content;
    public static int MaxamountofInstruments = 6;
    public static int AmountOFInstruments = 0;
    public static int minamountofInstruments = 1;

    Instrument[] arry;


    public void Start()
    {
        AmountOFInstruments = 0;

        StartCoroutine(LoadAll());
    }

    public IEnumerator LoadAll()
    {
        arry = Resources.LoadAll<Instrument>("Instruments/");

        for (int i = 0; i < arry.Length; i++)
        {
            GameObject go = Instantiate(ListObjectPreset, Content.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i * -100) - 50);
            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, i * 100);
            go.GetComponent<InstrumentButton>().name = arry[i].name;
        }
        yield return new WaitForSeconds(0);
    }
}
