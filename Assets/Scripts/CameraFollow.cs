using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothTime = 0.08f;
    public float rotationSpeed = 3.0f;

    [Header("Camera Offset Settings")]
    public float distance = 5.0f;
    public float height = 1.5f;

    private float currentX = 0f;
    private float currentY = 20f;
    private Vector3 _velocity = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void LateUpdate()
    {
        if (player == null) return;

        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentY = Mathf.Clamp(currentY, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = (rotation * new Vector3(0f, 0f, -distance))
                                 + player.position + Vector3.up * height;

        transform.position = Vector3.SmoothDamp(
            transform.position, targetPosition, ref _velocity, smoothTime);

        transform.LookAt(player.position + Vector3.up * height);
    }
}