using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float edgeBoundary = 10.0f;
    public float zoomSpeed = 2.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 20.0f;

    private Camera cam;
    private Vector2 moveInput;
    private Vector3 dragOrigin;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnDragStarted()
    {
        dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    public void OnDragPerformed()
    {
        Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        cam.transform.position += difference;
    }

    void MoveCamera()
    {
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0);
        if (Mouse.current.position.x.ReadValue() < edgeBoundary) move.x = -1;
        if (Mouse.current.position.x.ReadValue() > Screen.width - edgeBoundary) move.x = 1;
        if (Mouse.current.position.y.ReadValue() < edgeBoundary) move.y = -1;
        if (Mouse.current.position.y.ReadValue() > Screen.height - edgeBoundary) move.y = 1;

        cam.transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    void ZoomCamera()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
    }
}
