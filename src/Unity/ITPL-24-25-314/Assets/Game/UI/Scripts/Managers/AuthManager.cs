using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    private readonly string baseUrl = "http://g-martin.work:7002/auth";
    

    [System.Serializable]
    public class LoginRequest
    {
        public string Username;
        public string Password;
    }

    [System.Serializable]
    public class RegisterRequest
    {
        public string Username;
        public string Email;
        public string Password;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public string token;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var loginPayload = new LoginRequest { Username = username, Password = password };
        string json = JsonUtility.ToJson(loginPayload);

        using var client = new HttpClient();
        var response = await client.PostAsync($"{baseUrl}/login", new StringContent(json, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError($"Login failed: {response.StatusCode}");
            return null;
        }

        var body = await response.Content.ReadAsStringAsync();
        var auth = JsonUtility.FromJson<AuthResponse>(body);
        
        PlayerPrefs.SetString("username", username);          
        PlayerPrefs.SetString("custom_token", auth.token);    
        PlayerPrefs.Save();

        return auth.token;
    }


    public async Task<bool> RegisterAsync(string username, string email, string password)
    {
        var registerPayload = new RegisterRequest { Username = username, Email = email, Password = password };
        string json = JsonUtility.ToJson(registerPayload);

        using var client = new HttpClient();
        var response = await client.PostAsync($"{baseUrl}/register", new StringContent(json, Encoding.UTF8, "application/json"));

        return response.IsSuccessStatusCode;
    }
}