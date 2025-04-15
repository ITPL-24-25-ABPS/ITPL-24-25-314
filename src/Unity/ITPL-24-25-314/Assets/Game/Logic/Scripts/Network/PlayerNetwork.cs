using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] public Transform cameraTransform; // Reference to the camera
    [SerializeField] public float moveSpeed = 10f; // Speed of camera movement
    [SerializeField] public float zoomSpeed = 5f; // Speed of zoom
    [SerializeField] public float rotationSpeed = 50f; // Speed of angle adjustment
    [SerializeField] public float minZoom = 5f; // Minimum zoom distance
    [SerializeField] public float maxZoom = 20f; // Maximum zoom distance
    [SerializeField] public float minAngle = 20f; // Minimum camera angle
    [SerializeField] public float maxAngle = 80f; // Maximum camera angle

    [SerializeField] private float currentZoom = 10f; // Current zoom level
    [SerializeField] private float currentAngle = 45f; // Current camera angle
    
    private NetworkVariable<int> _playerScore = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    public override void OnNetworkSpawn(){
        _playerScore.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log("Player Score changed from " + previousValue + " to " + newValue);
            Debug.Log("Who: " + OwnerClientId  + ";  Player Score: " + _playerScore.Value);
        };
    }

    void Update()
    {
        if (IsOwner){
            HandleMovement();
            HandleZoom();
            HandleAngle();
            
            if (Input.GetKeyDown(KeyCode.Q)){
                _playerScore.Value++;
            }
        }
        
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.zero; // Initialize movement vector

        if (Input.GetKey(KeyCode.W)) movement += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) movement += Vector3.back;
        if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movement += Vector3.right;

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World); // Move the camera

    }

    void HandleZoom()
    {
        // Get the scroll wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed; // Adjust zoom based on scroll input
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom); // Clamp the zoom value

        cameraTransform.localPosition = new Vector3(0, currentZoom, -currentZoom); // Update camera position based on zoom
    }

    void HandleAngle()
    {
        if (Input.GetMouseButton(2)) // Check if the middle mouse button is held
        {
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseX = Input.GetAxis("Mouse X");

            // Adjust vertical angle (pitch)
            currentAngle -= mouseY * rotationSpeed * Time.deltaTime;
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

            // Adjust horizontal angle (yaw)
            float horizontalAngle = mouseX * rotationSpeed * Time.deltaTime;

            // Apply the rotation
            transform.Rotate(0, horizontalAngle, 0, Space.World); // Rotate the pivot horizontally
            cameraTransform.localRotation = Quaternion.Euler(currentAngle, 0, 0); // Rotate the camera vertically
        }
    }
}
