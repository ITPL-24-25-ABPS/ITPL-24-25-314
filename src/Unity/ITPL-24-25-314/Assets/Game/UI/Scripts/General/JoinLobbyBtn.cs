using UnityEngine;

namespace Game.UI.Scripts.General{
    public class JoinLobbyBtn : MonoBehaviour{
        public bool needPassword;
        public string lobbyId;

        public void JoinLobbyButtonPressed()
        {
            LobbyManager.Instance.JoinLobby(lobbyId, needPassword);
        }
    }
}