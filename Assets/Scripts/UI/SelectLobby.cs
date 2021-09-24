using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectLobby : MonoBehaviour
{
    public TextMeshProUGUI InfoText, LobbyKeyText;
    JoinLobby joinLobby;
    public string LobbyKey;
    public string message;
    public Color SelectedColor;
    public Color DeSelectColor;
    public Image Background;

    void Start()
    {
        joinLobby = FindObjectOfType<JoinLobby>();
        InfoText.text = message;
        LobbyKeyText.text = "Lobby: " + LobbyKey;
    }

    // Update is called once per frame
    public void Select()
    {
        Debug.Log("Select + " + LobbyKey);
        joinLobby.SetLobbyKey(LobbyKey);
        joinLobby.CheckLobby();
        Background.color = SelectedColor;
    }
}
