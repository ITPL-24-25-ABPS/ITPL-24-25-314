using System;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour{
    public static LobbyManager Instance { get; private set; }
    
    [Header("Start Screen")]
    [SerializeField] private GameObject startScreenParent;
    
    [Space(10)]
    [Header("Lobby Creation")] 
    [SerializeField] private GameObject lobbyCreationParent;
    [SerializeField] private TMP_InputField createLobbyNameInputField;
    [SerializeField] private TMP_Dropdown createLobbyMaxPlayersDropDownField;
    
    [Space(10)]
    [Header("Lobby list")]
    [SerializeField] private GameObject lobbyListParent;
    [SerializeField] private Transform lobbyContentParent;
    [SerializeField] private Transform lobbyItemPrefab;
    
    [Space(10)]
    [Header("Lobby Info")]
    [SerializeField] private GameObject lobbyInfoParent;
    
    [Space(10)]
    [Header("Joined lobby")]
    [SerializeField] private GameObject joinedLobbyParent;
    [SerializeField] private Transform playerItemPrefab;
    [SerializeField] private Transform playerListParent;
    [SerializeField] private TextMeshProUGUI joinedLobbyNameText;
    [SerializeField] private TextMeshProUGUI joinedLobbyGamemodeText;
    [SerializeField] private GameObject joinedLobbyStartButton;

    [Space(10)]
    [Header("Password protection")]
    [SerializeField] private Button inputPasswordButton;
    [SerializeField] private TMP_InputField inputPasswordField;
    [SerializeField] private GameObject inputPasswordParent;
    
    public string joinedLobbyId;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        startScreenParent.SetActive(true);
        
        Instance = this;
        
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
    }

    public async void ShowLobbies() {
        startScreenParent.SetActive(false);
        lobbyListParent.SetActive(true);
        while (Application.isPlaying && lobbyListParent.activeSelf){
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            foreach (Transform t in lobbyContentParent){
                Destroy(t.gameObject);
            }

            foreach (Lobby lobby in queryResponse.Results){
                Transform newLobbyItem = Instantiate(lobbyItemPrefab, lobbyContentParent);
                newLobbyItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbyItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            }
            await Task.Delay(1000);
        }
    }
    public void ShowLobbyCreation(){
        startScreenParent.SetActive(false);
        lobbyCreationParent.SetActive(true);
        lobbyListParent.SetActive(false);
    }

    // Update is called once per frame
    public async void CreateLobby()
    {
        if (!int.TryParse(createLobbyMaxPlayersDropDownField.options[createLobbyMaxPlayersDropDownField.value].text, out int maxPlayers)){
            Debug.LogWarning("Incorrect max players input");
            return;
        }

        Lobby createdLobby = null;
        try{
            createdLobby =
                await LobbyService.Instance.CreateLobbyAsync(createLobbyNameInputField.text, maxPlayers);
            joinedLobbyId = createdLobby.Id;
        }
        catch (LobbyServiceException e){
            Debug.Log(e);
        }
        lobbyCreationParent.SetActive(false);
        lobbyInfoParent.SetActive(false);
        lobbyListParent.SetActive(true);
    }

    public void StartGame(){
        // Load Main Game Scene
        SceneManager.LoadScene("MainGame");
    }
}
