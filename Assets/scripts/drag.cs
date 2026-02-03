using UnityEngine;

public class DragThrowRotate2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera mainCamera;

    private bool isDragging;
    private Vector2 targetPosition;
    private Vector2 lastMousePosition;
    private Vector2 throwVelocity;

    [Header("Throw Settings")]
    [SerializeField] private float throwMultiplier = 4f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 180f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true;

        isDragging = true;

        Vector2 mouseWorldPos = GetMouseWorldPosition();
        targetPosition = mouseWorldPos;
        lastMousePosition = mouseWorldPos;
    }

    private void OnMouseUp()
    {
        isDragging = false;
        rb.freezeRotation = false;
        rb.linearVelocity = throwVelocity * throwMultiplier;
    }

    private void Update()
    {
        if (!isDragging) return;

        Vector2 mouseWorldPos = GetMouseWorldPosition();
        targetPosition = mouseWorldPos;

        throwVelocity = (mouseWorldPos - lastMousePosition) / Time.deltaTime;
        lastMousePosition = mouseWorldPos;
    }

    private void FixedUpdate()
    {
        if (!isDragging) return;

        rb.MovePosition(targetPosition);

        float rotationInput = GetRotationInput();
        if (rotationInput == 0f) return;

        rb.MoveRotation(
            rb.rotation + rotationInput * rotateSpeed * Time.fixedDeltaTime
        );
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }

    private static float GetRotationInput()
    {
        if (Input.GetKey(KeyCode.Q)) return 1f;
        if (Input.GetKey(KeyCode.E)) return -1f;
        return 0f;
    }
}
