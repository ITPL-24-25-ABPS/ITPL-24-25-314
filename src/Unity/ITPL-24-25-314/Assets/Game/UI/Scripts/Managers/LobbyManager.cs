using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
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
    [SerializeField] private TMP_InputField createLobbyPasswordField;
    [SerializeField] private Toggle createLobbyPrivateToggle;
    
    [Space(10)]
    [Header("Lobby list")]
    [SerializeField] private GameObject lobbyListParent;
    [SerializeField] private Transform lobbyContentParent;
    [SerializeField] private Transform lobbyItemPrefab;
    [SerializeField] private TMP_InputField searchLobbyNameInputField;
    
    [Space(10)]
    [Header("Lobby Info")]
    [SerializeField] private GameObject lobbyInfoParent;
    
    [Space(10)]
    [Header("Joined lobby")]
    [SerializeField] private GameObject joinedLobbyParent;
    [SerializeField] private GameObject playerListParent;
    [SerializeField] private TextMeshProUGUI joinedLobbyNameText;
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
        Instance = this;
        
        createLobbyPrivateToggle.onValueChanged.AddListener(OnCreateLobbyPrivateToggle);
        
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
    }
    public void OnCreateLobbyPrivateToggle(bool value)
    {
        createLobbyPasswordField.gameObject.SetActive(value);
    }
    
    public async void JoinLobby(string lobbyID, bool needPassword)
    {
        try
        {
            var options = new JoinLobbyByIdOptions();

            if (needPassword)
            {
                string password = await InputPassword();
                options.Password = password;
            }

            var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, options);
            joinedLobbyId = lobbyID;

            if (!lobby.Data.ContainsKey("joinCode"))
            {
                Debug.LogError("joinCode missing from lobby data");
                return;
            }

            LobbyManager.cachedHostId = lobby.HostId;
            LobbyManager.cachedJoinCode = lobby.Data["joinCode"].Value;

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
        lobbyListParent.SetActive(false);
        joinedLobbyParent.SetActive(true);
        UpdateLobbyInfo();
    }

    
    private async Task<string> InputPassword()
    {
        bool waiting = true;
        inputPasswordParent.SetActive(true);

        while (waiting)
        {
            inputPasswordButton.onClick.AddListener(() => waiting = false);
            await Task.Yield();
        }

        inputPasswordParent.SetActive(false);
        return inputPasswordField.text;
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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            var options = new CreateLobbyOptions
            {
                IsPrivate = createLobbyPrivateToggle.isOn,
                Password = createLobbyPrivateToggle.isOn ? createLobbyPasswordField.text : null,
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };
            
            createdLobby =
                await LobbyService.Instance.CreateLobbyAsync(createLobbyNameInputField.text, maxPlayers, options);
            
            LobbyManager.cachedHostId = AuthenticationService.Instance.PlayerId;
            LobbyManager.cachedJoinCode = joinCode;
            
            joinedLobbyId = createdLobby.Id;
        }
        catch (LobbyServiceException e){
            Debug.Log(e);
        }
        
        LobbyHeartbeat(createdLobby);
        lobbyCreationParent.SetActive(false);
        lobbyInfoParent.SetActive(true);
        lobbyListParent.SetActive(false);
    }
    
    private bool isJoined = false;
    private async Task UpdateLobbyInfo()
    {
        while (Application.isPlaying && !string.IsNullOrEmpty(joinedLobbyId))
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);
                
                joinedLobbyNameText.text = lobby.Name;

                joinedLobbyStartButton.SetActive(AuthenticationService.Instance.PlayerId == lobby.HostId);

                foreach (Transform child in playerListParent.transform)
                {
                    Destroy(child.gameObject);
                }
                
                foreach (Player player in lobby.Players)
                {
                    GameObject playerItem = new GameObject("Player");
                    var text = playerItem.AddComponent<TextMeshProUGUI>();
                    text.fontSize = 24;
                    text.text = player.Data.TryGetValue("DisplayName", out var nameData)
                        ? nameData.Value
                        : "Unknown Player";
                    playerItem.transform.SetParent(playerListParent.transform, false);
                }

                await Task.Delay(1000);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning($"Lobby info update failed: {e}");
                break;
            }

            await Task.Delay(1000);
        }
    }


    public static string cachedJoinCode;
    public static string cachedHostId;
    public async void StartGame()
    {
        try
        {
            var lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);

            if (!lobby.Data.ContainsKey("joinCode"))
            {
                Debug.LogError("No join code found in lobby.");
                return;
            }

            cachedJoinCode = lobby.Data["joinCode"].Value;

            SceneManager.LoadScene("MainGame");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("StartGame failed: " + e);
        }
    }

    
    private async void LobbyHeartbeat(Lobby lobby)
    {
        while (true)
        {
            if (lobby == null)
            {
                return;
            }

            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);

            await Task.Delay(15 * 1000);
        }
    }
}