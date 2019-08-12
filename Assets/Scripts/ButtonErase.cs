using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonErase : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData data)
    {
        if (GameObject.FindGameObjectWithTag("NoteEditor").GetComponent<NoteEditor>().ErasingMode == true)
        {
            Destroy(this.gameObject);
        }
    }
}
