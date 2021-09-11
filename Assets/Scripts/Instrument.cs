using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Instrument : ScriptableObject
{
    public Sprite icon;
    public AudioClip[] samples;
    public bool affectedByPitch = true;
    public float decay = 4;
}
