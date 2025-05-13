using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Game.Logic.Scripts.Network
{
    public class PortSetupScript : MonoBehaviour
    {
        [Tooltip("Default port to use if --port is not specified")]
        public ushort defaultPort = 7777;

        void Awake()
        {
            ushort port = defaultPort;

            // Parse command line args
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "--port" || args[i] == "-port") && i + 1 < args.Length)
                {
                    if (ushort.TryParse(args[i + 1], out ushort parsedPort))
                    {
                        port = parsedPort;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid port number after --port");
                    }
                }
            }
            Debug.Log($"[Startup] Using network port: {port}");

            var transport = FindObjectOfType<UnityTransport>();
            if (transport != null)
            {
                transport.ConnectionData.Port = port;
                Debug.Log($"✅ Set TelepathyTransport to port {port}");
            }
            else
            {
                Debug.LogWarning("❌ Could not find TelepathyTransport component");
            }
        }

        void Update()
        {
        
        }
    }
}
