using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEditor : MonoBehaviour
{
    public GameObject NoteToPlace;

    public GameObject NotesParent;

    public RectTransform Viewport;

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

        xMin = Viewport.position.x;
        xMax = Viewport.rect.width;
        yMin = Viewport.position.y - Viewport.rect.height;
        yMax = Viewport.position.y;

        if (PlacingMode == true)
        {
            if (MousePosition.x > xMin && MousePosition.x < xMax && MousePosition.y > yMin && MousePosition.y < yMax)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject PlacableNote = Instantiate<GameObject>(NoteToPlace);
                    PlacableNote.transform.SetParent(MusicGrid.transform);
                    PlacableNote.transform.localScale = Vector3.one;
                    PlacableNote.transform.position = MousePosition;
                }
            }

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
