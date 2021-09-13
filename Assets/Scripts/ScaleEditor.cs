using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleEditor : MonoBehaviour
{
    public bool maximized;

    public RectTransform keyboard;
    public RectTransform editor;
    public RectTransform viewPort;
    public RectTransform viewPortContent;
    public RectTransform counter;

    public float normalHeight = 375f;
    public float maxHeight = 775f;
    float height;

    public float normalEditorHeight = 650f;
    public float maxEditorHeight = 1065f;
    float editorHeight;

    float keyboardHeight;
    float keyboardStartPosY;

    public Scrollbar scrollbarHorizontal;
    public Scrollbar scrollbarVertical;

    public float[] gridSizes;
    int xGridSizeIndex, yGridSizeIndex;

    void Start()
    {
        height = maximized ? maxHeight : normalHeight;
        editorHeight = maximized ? maxEditorHeight : normalEditorHeight;
        keyboardStartPosY = keyboard.localPosition.y;
        keyboardHeight = keyboardStartPosY;

        xGridSizeIndex = 0;
        yGridSizeIndex = 0;
    }

    void Update()
    {
        GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(GetComponent<RectTransform>().sizeDelta, new Vector2(GetComponent<RectTransform>().sizeDelta.x, height), 4 * Time.deltaTime);
        viewPort.sizeDelta = new Vector2(viewPort.sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);

        editor.sizeDelta = Vector2.Lerp(editor.sizeDelta, new Vector2(editor.sizeDelta.x, editorHeight), 4 * Time.deltaTime);

        keyboardHeight = Mathf.Lerp(keyboardHeight, maximized ? -editorHeight / 2 - 10f : keyboardStartPosY, 4 * Time.deltaTime);
        keyboard.localPosition = new Vector3(keyboard.localPosition.x, keyboardHeight, keyboard.localPosition.z);
    }

    public void ChangeSize()
    {
        scrollbarHorizontal.value = Mathf.Clamp01(scrollbarHorizontal.value);
        scrollbarVertical.value = Mathf.Clamp01(scrollbarVertical.value);

        maximized = !maximized;

        height = maximized ? maxHeight : normalHeight;
        editorHeight = maximized ? maxEditorHeight : normalEditorHeight;
    }

    public void ChangeGridSizeX()
    {
        if (xGridSizeIndex < gridSizes.Length - 1)
        {
            xGridSizeIndex++;
        }
        else
        {
            xGridSizeIndex = 0;
        }

        UpdateGridSize();
    }

    public void ChangeGridSizeY()
    {
        if (yGridSizeIndex < gridSizes.Length - 1)
        {
            yGridSizeIndex++;
        }
        else
        {
            yGridSizeIndex = 0;
        }

        UpdateGridSize();
    }

    void UpdateGridSize()
    {
        viewPortContent.localScale = new Vector3(gridSizes[xGridSizeIndex], gridSizes[yGridSizeIndex], 1);
        counter.localScale = new Vector3(gridSizes[0] / gridSizes[xGridSizeIndex], gridSizes[0] / gridSizes[yGridSizeIndex], 1);

        scrollbarHorizontal.value = Mathf.Clamp01(scrollbarHorizontal.value);
        scrollbarVertical.value = Mathf.Clamp01(scrollbarVertical.value);
    }
}
