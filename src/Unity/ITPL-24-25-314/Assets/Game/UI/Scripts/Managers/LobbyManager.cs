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

public class LobbyManager : MonoBehaviour{
    public static LobbyManager Instance { get; private set; }
    
    [Header("Lobby Creation")] 
    [SerializeField] private GameObject lobbyCreationParent;
    [SerializeField] private TMP_InputField createLobbyNameInputField;
    [SerializeField] private TMP_Dropdown createLobbyMaxPlayersDropDownField;
    
    [Space(10)]
    [Header("Lobby list")]
    [SerializeField] private GameObject lobbyListParent;
    [SerializeField] private Transform lobbyContentParent;
    [SerializeField] private Transform lobbyItemPrefab;
    
    public string joinedLobbyId;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        Instance = this;
        
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void ShowLobbies() {
        while (Application.isPlaying && lobbyCreationParent.activeInHierarchy){
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (Transform t in lobbyContentParent){
                Destroy(t.gameObject);
            }

            foreach (Lobby lobby in queryResponse.Results){
                Transform newLobbyItem = Instantiate(lobbyItemPrefab, lobbyContentParent);
            }
            await Task.Delay(1000);
        }
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
        lobbyListParent.SetActive(true);
        ShowLobbies();
    }
}
