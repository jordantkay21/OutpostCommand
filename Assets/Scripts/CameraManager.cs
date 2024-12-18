using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineVirtualCamera virtualCamera; // Reference to the virtual camera

    [Header("Movement Settings")]
    public float moveSpeed = 10f; // Movement speed of the camera
    public float zoomSpeed = 20f; // Zooming speed
    public float minZoom = 30f;   // Minimum FOV (zoomed in)
    public float maxZoom = 80f;   // Maximum FOV (zoomed out)

    private InputActions inputActions; // Input Actions instance
    private Vector2 moveInput;         // Stores input for camera movement
    private float zoomInput;           // Stores input for zoom

    private Transform cameraTransform; // Transform of the virtual camera

    private void Awake()
    {
        inputActions = new InputActions(); // Initialize Input Actions
        cameraTransform = virtualCamera.transform; // Get the virtual camera transform
    }

    private void OnEnable()
    {
        // Enable input and subscribe to actions
        inputActions.Camera.Enable();
        inputActions.Camera.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Camera.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Camera.Zoom.performed += ctx => zoomInput = ctx.ReadValue<float>();
        inputActions.Camera.Zoom.canceled += ctx => zoomInput = 0f;
    }

    private void OnDisable()
    {
        inputActions.Camera.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        // Move the camera based on WSAD/Arrow input
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        cameraTransform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        // Adjust the Field of View for zooming
        float currentFOV = virtualCamera.m_Lens.FieldOfView;
        currentFOV -= zoomInput * zoomSpeed * Time.deltaTime;
        currentFOV = Mathf.Clamp(currentFOV, minZoom, maxZoom);

        virtualCamera.m_Lens.FieldOfView = currentFOV;
    }
}
