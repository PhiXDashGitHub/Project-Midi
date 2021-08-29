using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEditor : MonoBehaviour
{
    public bool maximized;

    public RectTransform keyboard;
    public RectTransform editor;
    public RectTransform viewPort;

    public float normalHeight = 375f;
    public float maxHeight = 775f;
    float height;

    public float normalEditorHeight = 650f;
    public float maxEditorHeight = 1065f;
    float editorHeight;

    float keyboardHeight;
    float keyboardStartPosY;

    void Start()
    {
        height = maximized ? maxHeight : normalHeight;
        editorHeight = maximized ? maxEditorHeight : normalEditorHeight;
        keyboardStartPosY = keyboard.localPosition.y;
        keyboardHeight = keyboardStartPosY;
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
        maximized = !maximized;

        height = maximized ? maxHeight : normalHeight;
        editorHeight = maximized ? maxEditorHeight : normalEditorHeight;
    }
}
