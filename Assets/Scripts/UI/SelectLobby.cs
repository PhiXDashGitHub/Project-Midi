using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectLobby : MonoBehaviour
{
    [Header("Settings")]
    JoinLobby joinLobby;
    public string LobbyKey;
    public string message;

    [Header("UI")]
    public TextMeshProUGUI InfoText, LobbyKeyText;
    public Color SelectedColor;
    public Color DeSelectColor;
    public Image Background;

    void Start()
    {
        joinLobby = FindObjectOfType<JoinLobby>();
        InfoText.text = message;
        LobbyKeyText.text = "Lobby: " + LobbyKey;
    }

    public void Select()
    {
        Debug.Log("Select + " + LobbyKey);
        joinLobby.SetLobbyKey(LobbyKey);
        joinLobby.CheckLobby();
        Background.color = SelectedColor;
    }
}
