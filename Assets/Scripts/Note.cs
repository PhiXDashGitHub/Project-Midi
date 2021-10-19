using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Note : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public bool mouseOver;
    [HideInInspector] public bool dragging;
    bool resizeing;

    public int instrument;
    public int value;
    public float pos;
    public float length;

    RectTransform rectTransform;
    public Transform baseTransform;
    public Transform lengthTransform;

    public Image baseImage;
    public Image lengthImage;

    UIColors uiColors;

    Scrollbar horizontalScrollbar;
    Scrollbar verticalScrollbar;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        dragging = false;
        mouseOver = false;
        resizeing = false;

        uiColors = Resources.LoadAll<UIColors>("UI/")[0];
        Color baseColor = uiColors.noteColors[instrument];
        baseColor = Color.Lerp(baseColor, Color.white, 0.33f);

        baseImage.color = baseColor;
        lengthImage.color = uiColors.noteColors[instrument];

        UpdateTransform(length);

        horizontalScrollbar = FindObjectsOfType<Scrollbar>()[0];
        verticalScrollbar = FindObjectsOfType<Scrollbar>()[1];

        horizontalScrollbar.onValueChanged.AddListener(StopModifying);
        verticalScrollbar.onValueChanged.AddListener(StopModifying);
    }

    void Update()
    {
        if (mouseOver)
        {
            if (Input.GetMouseButton(0) && NoteEditor.editMode == NoteEditor.EditMode.None)
            {
                Vector3 mouseOffset = transform.InverseTransformPoint(Input.mousePosition) + Vector3Int.up;
                NoteEditor.lastNoteLength = length;

                if (!dragging && (mouseOffset.x > baseTransform.localScale.x || resizeing))
                {
                    resizeing = true;
                    dragging = false;

                    UpdateTransform(mouseOffset.x);
                }
                else if (!resizeing)
                {
                    dragging = true;
                    resizeing = false;

                    Vector3 localPos = transform.localPosition;
                    localPos += mouseOffset;

                    localPos.x = Mathf.FloorToInt(localPos.x / NoteEditor.s_gridSize) * NoteEditor.s_gridSize;
                    localPos.y = Mathf.FloorToInt(localPos.y);

                    localPos.x = Mathf.Clamp(localPos.x, 0, 332);
                    localPos.y = Mathf.Clamp(localPos.y, 0, NoteEditor.s_keyRange);
                    transform.localPosition = localPos;

                    value = (int)transform.localPosition.y;
                    pos = localPos.x;
                }
            }
            else if (Input.GetMouseButton(0) && NoteEditor.editMode == NoteEditor.EditMode.Erase)
            {
                Destroy(gameObject);
            }
        }

        if (Input.GetMouseButtonUp(0) && NoteEditor.editMode != NoteEditor.EditMode.Place)
        {
            dragging = false;
            resizeing = false;
            mouseOver = false;
        }
    }

    public void UpdateTransform(float mouseOffset)
    {
        length = Mathf.Clamp(mouseOffset, 1, mouseOffset);
        rectTransform.sizeDelta = new Vector2(length, rectTransform.sizeDelta.y);

        Vector3 baseScale = baseTransform.localScale;
        baseScale.x = rectTransform.sizeDelta.x - 0.5f;
        baseScale.x = Mathf.Clamp(baseScale.x, 0.5f, baseScale.x);
        baseTransform.localScale = baseScale;

        Vector3 lengthLocalPos = lengthTransform.localPosition;
        lengthLocalPos.x = baseScale.x;
        lengthTransform.localPosition = lengthLocalPos;

        value = (int)transform.localPosition.y;
        pos = Mathf.FloorToInt(transform.localPosition.x / NoteEditor.s_gridSize) * NoteEditor.s_gridSize;
    }

    public void UpdatePosition()
    {
        Vector3 localPos = transform.localPosition;
        localPos.x = Mathf.Clamp(localPos.x, 0, 332);
        localPos.x = Mathf.FloorToInt(localPos.x / NoteEditor.s_gridSize) * NoteEditor.s_gridSize;
        localPos.y = Mathf.Clamp(localPos.y, 0, NoteEditor.s_keyRange);
        localPos.y = Mathf.FloorToInt(localPos.y);
        transform.localPosition = localPos;

        value = (int)transform.localPosition.y;
        pos = Mathf.FloorToInt(transform.localPosition.x / NoteEditor.s_gridSize) * NoteEditor.s_gridSize;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (Input.touchCount > 1)
        {
            return;
        }

        if (NoteEditor.editMode == NoteEditor.EditMode.Place)
        {
            mouseOver = true;
            NoteEditor.lastNoteLength = length;
            return;
        }
        else if (dragging || DraggingOtherNote())
        {
            return;
        }

        mouseOver = true;

        length = GetComponent<RectTransform>().rect.width;
    }

    bool DraggingOtherNote()
    {
        foreach (Note note in transform.parent.GetComponentsInChildren<Note>())
        {
            if (note != this)
            {
                if (note.dragging)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!dragging && !resizeing)
        {
            mouseOver = false;
        }
    }

    public void StopModifying(float value)
    {
        mouseOver = false;
        dragging = false;
        resizeing = false;
    }
}
