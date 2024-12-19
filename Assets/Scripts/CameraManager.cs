using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineVirtualCamera virtualCamera; // Reference to the virtual camera
    public Transform cameraHolder;                // Parent object for rotation

    [Header("Movement Settings")]
    public float moveSpeed = 10f;  // Movement speed
    public float zoomSpeed = 20f;  // Zoom speed
    public float minZoom = 30f;    // Minimum FOV
    public float maxZoom = 80f;    // Maximum FOV

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f; // Rotation speed when holding the right mouse button
    private float pitchAngle = 0f; // Track the current pitch angle
    public float minPitch = -30f; // Minimum pitch angle (downwards)
    public float maxPitch = 60f;  // Maximum pitch angle (upwards)
    public bool isRotating;           // Tracks if the right mouse button is held

    private InputActions inputActions; // Input Actions instance
    private Vector2 moveInput;         // Stores input for camera movement
    private float zoomInput;           // Stores input for zoom
    private Vector2 rotationInput;     // Stores input for camera rotation

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

        inputActions.Camera.Rotate.performed += ctx => isRotating = ctx.ReadValue<float>() > 0;
        inputActions.Camera.Rotate.canceled += ctx => isRotating = false;

        inputActions.Camera.MouseDelta.performed += ctx => rotationInput = ctx.ReadValue<Vector2>();
        inputActions.Camera.MouseDelta.canceled += ctx => rotationInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Camera.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Get the forward and right directions relative to the camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten the forward and right vectors to ignore vertical movement
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate the movement direction based on input
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // Move the camera
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

    private void HandleRotation()
    {
        if (isRotating)
        {
            // Rotate the camera based on mouse movement (delta)
            float yaw = rotationInput.x * rotationSpeed * Time.deltaTime; // Horizontal rotation
            float pitch = rotationInput.y * rotationSpeed * Time.deltaTime; // Vertical rotation

            // Apply horizontal rotation (yaw)
            cameraTransform.Rotate(Vector3.up, yaw, Space.World);

            // Adjust pitch angle and clamp it
            pitchAngle -= pitch; // Invert pitch for natural camera movement
            pitchAngle = Mathf.Clamp(pitchAngle, minPitch, maxPitch);

            // Apply vertical rotation (pitch)
            Vector3 currentRotation = cameraTransform.eulerAngles;
            currentRotation.x = pitchAngle;
            cameraTransform.eulerAngles = currentRotation;
        }
    }
}
