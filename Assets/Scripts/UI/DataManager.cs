using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public NoteEditor noteEditor;
    public UIManager uiManager;

    public GameObject editor;
    public GameObject currentMenu;

    List<string> savedSongs;
    public GameObject songSelectionPrefab;
    public RectTransform scrollViewContent;
    public TMP_InputField fileNameInput;
    public TextMeshProUGUI errorText;

    string filePath;
    string fileType;

    string selectedFile;

    void Start()
    {
        filePath = Application.persistentDataPath + "/Songs/";
        fileType = ".ndf";

        savedSongs = new List<string>();

        FindSongs();
    }

    public void FindSongs()
    {
        savedSongs = new List<string>();

        foreach (Transform content in scrollViewContent)
        {
            Destroy(content.gameObject);
        }

        if (Directory.Exists(filePath))
        {
            string[] files = Directory.GetFiles(filePath);

            foreach (string file in files)
            {
                if (file.EndsWith(fileType))
                {
                    string baseName = file.Replace(filePath, "").Replace(fileType, "").Trim();

                    savedSongs.Add(baseName);

                    GameObject newButton = Instantiate(songSelectionPrefab, scrollViewContent);
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = baseName;
                    newButton.GetComponent<Button>().onClick.AddListener(() => SelectFile(newButton.GetComponent<Button>(), baseName));
                }
            }

            scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, 100 * savedSongs.Count);
        }
    }

    public void SelectFile(Button button, string name)
    {
        button.Select();
        selectedFile = name;
    }

    public void DeselectFile()
    {
        selectedFile = null;
    }

    public void SaveFile()
    {
        if (fileNameInput.text == null)
        {
            return;
        }

        if (!File.Exists(filePath + fileNameInput.text + fileType))
        {
            uiManager.ActivateFade(editor);
            uiManager.DeActivateDelay(currentMenu);

            StartCoroutine(SaveDelayed(fileNameInput.text));
            errorText.enabled = false;
        }
        else
        {
            errorText.enabled = true;
        }
    }

    public void LoadFile()
    {
        if (selectedFile == null)
        {
            return;
        }

        uiManager.ActivateFade(editor);
        uiManager.DeActivateDelay(currentMenu);

        StartCoroutine(LoadDelayed(selectedFile));

        selectedFile = null;
    }

    public void DeleteFile()
    {
        if (selectedFile == null)
        {
            return;
        }

        File.Delete(filePath + selectedFile + fileType);

        selectedFile = null;
        FindSongs();
    }

    IEnumerator LoadDelayed(string name)
    {
        yield return new WaitForSeconds(0.45f);
        noteEditor.LoadFromFile(name);
    }

    IEnumerator SaveDelayed(string name)
    {
        yield return new WaitForSeconds(0.45f);
        noteEditor.SaveToFile(name);
    }
}
