using UnityEngine;
using TMPro; // 1. STEP: Necessary to access UI components

public class PlayerMovement : MonoBehaviour
{
    // --- UI REFERENCES ---
    [Header("UI Settings")]
    public TextMeshProUGUI gemText; // Slot for our GemCounter object

    // --- MOVEMENT SETTINGS ---
    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 5.0f;

    // --- PLAYER DATA ---
    [Header("Player Data")]
    public int score = 0;

    private Rigidbody rb;

    // --- INITIALIZATION ---
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure the screen displays "Gems: 0" at the start
        UpdateUI();
    }

    void Update()
    {
        HandleMovement();
    }

    // --- COLLISION LOGIC ---
    private void OnTriggerEnter(Collider other)
    {
        // Detect if the object we touched is a Gem
        if (other.gameObject.CompareTag("Gem"))
        {
            score += 10;        // Add points
            UpdateUI();         // Refresh the text on the screen
            Destroy(other.gameObject); // Remove gem from the world
        }
    }

    // --- UI HELPER METHOD ---
    void UpdateUI()
    {
        // Format the message for the player (Visualizing the data)
        gemText.text = "Gems: " + score;
    }

    void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2 : speed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.Translate(x * currentSpeed * Time.deltaTime, 0, z * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}