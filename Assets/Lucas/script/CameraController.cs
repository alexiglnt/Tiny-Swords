using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public Vector2 panLimit;
    public float zoomSpeed = 20f;
    public float scrollZoomSensitivity = 0.1f; // Sensitivity for the scroll wheel zoom
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float dragSensitivity = 0.005f;
    
    private Controls controls;

    private Vector2 moveInput;
    private float zoomInput;

    private Vector2 lastMousePosition;


    private void Awake()
    {
        controls = new Controls();

        // Zoom
        controls.Gameplay.ZoomIn.performed += ctx => SetZoomInput(ctx.ReadValue<float>(), true);
        controls.Gameplay.ZoomIn.canceled += ctx => SetZoomInput(0, true);
        controls.Gameplay.ZoomOut.performed += ctx => SetZoomInput(ctx.ReadValue<float>(), false);
        controls.Gameplay.ZoomOut.canceled += ctx => SetZoomInput(0, false);
        controls.Gameplay.Scroll.performed += ctx => SetScrollZoomInput(ctx.ReadValue<Vector2>().y);
        controls.Gameplay.Scroll.canceled += ctx => SetScrollZoomInput(0);

        // Movement
        controls.Gameplay.Up.performed += ctx => SetMoveInput(Vector2.up);
        controls.Gameplay.Up.canceled += ctx => ResetMoveInput();
        controls.Gameplay.Down.performed += ctx => SetMoveInput(Vector2.down);
        controls.Gameplay.Down.canceled += ctx => ResetMoveInput();
        controls.Gameplay.Left.performed += ctx => SetMoveInput(Vector2.left);
        controls.Gameplay.Left.canceled += ctx => ResetMoveInput();
        controls.Gameplay.Right.performed += ctx => SetMoveInput(Vector2.right);
        controls.Gameplay.Right.canceled += ctx => ResetMoveInput();
        controls.Gameplay.Drag.performed += ctx => OnDragStarted(ctx);
        controls.Gameplay.Drag.canceled += ctx => OnDragEnded(ctx);
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // Handle camera movement input
        pos.x += moveInput.x * panSpeed * Time.deltaTime;
        pos.y += moveInput.y * panSpeed * Time.deltaTime;

        // Handle camera zoom input
        // Combine button zoom inputs and scroll wheel zoom input
        float totalZoomInput = zoomInput + scrollInput;
        Camera.main.orthographicSize -= totalZoomInput * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);


        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 delta = Mouse.current.position.ReadValue() - lastMousePosition;
            // Apply drag sensitivity
            delta *= dragSensitivity;
            // Transform delta from screen to world coordinates
            Vector3 worldDelta = Camera.main.ScreenToWorldPoint(new Vector3(delta.x, delta.y, Camera.main.nearClipPlane)) - 
                                 Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            // Update camera position
            pos -= worldDelta;
            lastMousePosition = Mouse.current.position.ReadValue();
        }

        // Clamp camera position to pan limits
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);

        transform.position = pos;
    }


    private void ResetMoveInput()
    {
        // Reset the moveInput if no keys are pressed.
        if (!controls.Gameplay.Up.IsPressed() &&
            !controls.Gameplay.Down.IsPressed() &&
            !controls.Gameplay.Left.IsPressed() &&
            !controls.Gameplay.Right.IsPressed())
        {
            moveInput = Vector2.zero;
        }
        // You may also need to check for currently pressed keys to set the moveInput accordingly.
        else
        {
            moveInput = new Vector2(
                (controls.Gameplay.Right.IsPressed() ? 1 : 0) - (controls.Gameplay.Left.IsPressed() ? 1 : 0),
                (controls.Gameplay.Up.IsPressed() ? 1 : 0) - (controls.Gameplay.Down.IsPressed() ? 1 : 0)
            );
        }
    }

    private void SetMoveInput(Vector2 direction)
    {
        moveInput = direction;
    }
    
    private float zoomInInput = 0f;
    private float zoomOutInput = 0f;
    private float scrollInput = 0f;
    private void SetZoomInput(float zoomDirection, bool isZoomingIn)
    {
        // Button zoom inputs
        if (isZoomingIn)
        {
            zoomInInput = zoomDirection;
        }
        else
        {
            zoomOutInput = zoomDirection;
        }
        // Combine zoom in and zoom out inputs
        zoomInput = zoomInInput - zoomOutInput;
    }

    // Add this method for scroll wheel input
    private void SetScrollZoomInput(float scrollValue)
    {
        scrollInput = scrollValue * scrollZoomSensitivity;
    }

    private void OnDragStarted(InputAction.CallbackContext context)
    {
        lastMousePosition = Mouse.current.position.ReadValue();
    }

    private void OnDragEnded(InputAction.CallbackContext context)
    {
        // This can be left empty if you have nothing specific to do when the drag ends
    }

}