using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Scripts.Managers{
    public class Register : MonoBehaviour{
            
            [Space(10)]
            [Header("RegisterUi")]
            [SerializeField] private TMP_InputField UsernameInput;
            [SerializeField] private TMP_InputField EmailInput;
            [SerializeField] private TMP_InputField PasswordInput;
            [SerializeField] private TMP_Text StatusText;
            [SerializeField] private Button RegisterButton;
            [SerializeField] private GameObject RegisterPanel;
    
            [Space(10)]
            [Header("AuthManager")]
            [SerializeField] private AuthManager authManager;
            
            [Space(10)]
            [Header("LoginPanel")]
            [SerializeField] private GameObject LoginPanel;
    
            void Start()
            {
                RegisterButton.onClick.AddListener(OnRegisterClicked);
            }
    
            public async void OnRegisterClicked()
            {
                string username = UsernameInput.text;
                string email = EmailInput.text;
                string password = PasswordInput.text;
    
                StatusText.text = "Registering...";
    
                bool success = await authManager.RegisterAsync(username, email, password);
    
                if (success)
                {
                    StatusText.text = "Registration successful!";
                    RegisterPanel.SetActive(false);
                    LoginPanel.SetActive(true);
                }
                else
                {
                    StatusText.text = "Registration failed. Check details.";
                }
            }
    }
}