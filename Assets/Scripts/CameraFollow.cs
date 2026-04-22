using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    [Tooltip("Seconds to reach target position. Lower = snappier. 0.05–0.15 recommended.")]
    public float smoothTime = 0.08f;
    public float rotationSpeed = 3.0f;

    [Header("Camera Offset Settings")]
    public float distance = 0.83f;
    public float height = 0.33f;

    private float currentX = 0f;
    private float currentY = 0f;
    private Vector3 camVelocity = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    // Fiziksel titremeyi önlemek için FixedUpdate sonrası en stabil yer burasıdır
    void LateUpdate()
    {
        if (player == null) return;

        // 1. Mouse Inputs
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentY = Mathf.Clamp(currentY, -40f, 85f);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // 2. Target Position Calculation
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        // Karakterin tam pozisyonu + rotasyonlu mesafe + yükseklik
        Vector3 targetPosition = (rotation * negDistance) + player.position + Vector3.up * height;

        // 3. Smooth Following — SmoothDamp is frame-rate independent and velocity-aware
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref camVelocity, smoothTime);

        // 4. Always look at the player's head area
        transform.LookAt(player.position + Vector3.up * height);
    }
}