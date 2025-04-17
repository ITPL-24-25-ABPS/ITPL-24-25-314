using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Testing : MonoBehaviour{
    [SerializeField] public Button startHost;
    [SerializeField] public Button startServer;
    [SerializeField] public Button startClient;
    // Update is called once per frame
    void Awake()
    {
        startHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        startServer.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        startClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
