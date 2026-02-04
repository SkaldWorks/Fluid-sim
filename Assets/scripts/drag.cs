using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class DragThrowRotate2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera mainCamera;

    private bool isDragging;
    private Vector2 lastMousePosition;
    private Vector2 throwVelocity;

    [SerializeField] private float throwMultiplier = 4f;
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
        lastMousePosition = GetMouseWorldPosition();
        throwVelocity = Vector2.zero;
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

        Vector2 mousePos = GetMouseWorldPosition();

        // Cache throw velocity once per frame (cheaper & more stable)
        throwVelocity = (mousePos - lastMousePosition) / Time.deltaTime;
        lastMousePosition = mousePos;
    }

    private void FixedUpdate()
    {
        if (!isDragging) return;

        rb.MovePosition(lastMousePosition);

        float rotationInput = GetRotationInput();
        if (rotationInput != 0f)
        {
            rb.MoveRotation(
                rb.rotation + rotationInput * rotateSpeed * Time.fixedDeltaTime
            );
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(pos.x, pos.y);
    }

    private static float GetRotationInput()
    {
        if (Input.GetKey(KeyCode.Q)) return 1f;
        if (Input.GetKey(KeyCode.E)) return -1f;
        return 0f;
    }
}
