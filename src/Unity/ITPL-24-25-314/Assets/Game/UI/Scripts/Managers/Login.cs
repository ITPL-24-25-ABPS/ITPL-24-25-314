using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Scripts.Managers{
    public class Login : MonoBehaviour{
        
        [Space(10)]
        [Header("LoginUi")]
        [SerializeField] private TMP_InputField UsernameInput;
        [SerializeField] private TMP_InputField PasswordInput;
        [SerializeField] private TMP_Text StatusText;
        [SerializeField] private Button LoginButton;
        [SerializeField] private Button RegisterInstedButton;
        [SerializeField] private GameObject LoginPanel;
        
        [Space(10)]
        [Header("AuthManager")]
        [SerializeField] private AuthManager authManager;
        
        [Space(10)]
        [Header("StartScreen")]
        [SerializeField] private GameObject startScreenParent;
        
        [Space(10)]
        [Header("RegisterPanel")]
        [SerializeField] private GameObject RegisterPanel;
        
        void Start()
        {
            LoginPanel.SetActive(true);
            LoginButton.onClick.AddListener(OnLoginClicked);
        }

        public async void OnLoginClicked()
        {
            string username = UsernameInput.text;
            string password = PasswordInput.text;

            StatusText.text = "Logging in...";

            string token = await authManager.LoginAsync(username, password);

            if (!string.IsNullOrEmpty(token))
            {
                StatusText.text = "Login successful!";
                LoginPanel.SetActive(false);
                startScreenParent.SetActive(true);
            }
            else
            {
                StatusText.text = "Login failed. Check credentials.";
            }
        }

        public void OnRegisterInstedClicked(){
            RegisterPanel.SetActive(true);
            LoginPanel.SetActive(false);
        }
    }
}