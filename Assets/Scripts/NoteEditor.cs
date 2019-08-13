using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEditor : MonoBehaviour
{
    public GameObject NoteToPlace;


    public GameObject NotesParent;

    GameObject MusicGrid;

    public Button PencilButton;
    public Button EraserButton;

    public Vector2 MousePosition;

    public bool PlacingMode;
    public bool ErasingMode;

    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    public float xGridSize;
    public float yGridSize;

    void Update()
    {
        for (int i = 0; i < NotesParent.transform.childCount; i++)
        {
            if (NotesParent.transform.GetChild(i).name == PlayerPrefs.GetString("CurrentInstrument"))
            {
                NotesParent.transform.GetChild(i).gameObject.SetActive(true);
                MusicGrid = NotesParent.transform.GetChild(i).gameObject;
            }
            else
            {
                NotesParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        MousePosition = Input.mousePosition;

        float xReciprocalGridSize = 1f / xGridSize;
        float yReciprocalGridSize = 1f / yGridSize;

        float xGrid = Mathf.Round(Mathf.Round(Input.mousePosition.x) * xReciprocalGridSize) / xReciprocalGridSize;
        float yGrid = Mathf.Round(Mathf.Round(Input.mousePosition.y) * yReciprocalGridSize) / yReciprocalGridSize;

        Vector2 GridMousePosition = new Vector2(Mathf.Round(xGrid), yGrid);

        float testmousepositionx = Input.mousePosition.x;
        float testmousepositiony = Input.mousePosition.y;

        if (PlacingMode == true)
        {
            //if (MousePosition.x > xMin && MousePosition.x < xMax && MousePosition.y > yMin && MousePosition.y < yMax)
            //{
            if((testmousepositionx > xMin) && (testmousepositionx < xMax))
            {
                if ((testmousepositiony > yMin )&& (testmousepositiony < yMax))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject PlacableNote = Instantiate<GameObject>(NoteToPlace);
                        PlacableNote.transform.SetParent(MusicGrid.transform);
                        PlacableNote.transform.localScale = Vector3.one;
                        PlacableNote.transform.position = GridMousePosition;
                    }
                }
            }
            //}

            PencilButton.image.color = Color.green;
        }
        else if (ErasingMode == true)
        {
            EraserButton.image.color = Color.green;
        }

        if (PlacingMode == false)
        {
            PencilButton.image.color = Color.white;
        }

        if (ErasingMode == false)
        {
            EraserButton.image.color = Color.white;
        }
    }

    public void ChangePlacingMode()
    {
        PlacingMode = !PlacingMode;
        ErasingMode = false;
    }

    public void ChangeErasingMode()
    {
        ErasingMode = !ErasingMode;
        PlacingMode = false;
    }
}
