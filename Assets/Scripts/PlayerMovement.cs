using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("UI & HUD Settings")]
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI infoText;

    [Header("Inventory & Hand")]
    public Transform playerHand; // Drag the "Hand" object here
    public bool hasKey = false;
    private GameObject heldKey;

    [Header("Player Stats")]
    public int score = 0;
    public int health = 100;

    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateUI();
        ShowInfo("Explore the dungeon and find the exit!");
    }

    void Update()
    {
        HandleMovement();
        if (health <= 0) RestartGame();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. PICKING UP THE KEY
        if (other.gameObject.CompareTag("Key") && !hasKey)
        {
            hasKey = true;
            heldKey = other.gameObject;

            // Parent the key to the hand
            heldKey.transform.SetParent(playerHand);
            heldKey.transform.localPosition = Vector3.zero;

            // Rotate key to face forward (90 degrees on Y)
            heldKey.transform.localRotation = Quaternion.Euler(0, 90f, 0);

            // Disable collider so it doesn't bump the player
            heldKey.GetComponent<Collider>().enabled = false;

            ShowInfo("Key Collected! Find the Iron Door.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 2. UNLOCKING THE DOOR (Z-AXIS -90 DEGREES)
        if (collision.gameObject.CompareTag("IronDoor"))
        {
            if (hasKey)
            {
                ShowInfo("The Door is opening...");

                Transform doorTrans = collision.transform;

                // --- PRECISION ROTATION ---
                // We preserve current X and Y, and ONLY set Z to -90
                float currentX = doorTrans.eulerAngles.x;
                float currentY = doorTrans.eulerAngles.y;
                doorTrans.rotation = Quaternion.Euler(currentX, currentY, -90f);

                // Turn into trigger to let player pass
                collision.collider.isTrigger = true;

                // Key is used up
                Destroy(heldKey);
                hasKey = false;
            }
            else
            {
                ShowInfo("Locked! You need a Golden Key.");
            }
        }
    }

    // --- HELPER METHODS ---

    void ShowInfo(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
            CancelInvoke("ClearText");
            Invoke("ClearText", 3.0f); // Clear message after 3 seconds
        }
    }

    void ClearText() { if (infoText != null) infoText.text = ""; }

    void TakeDamage(int amount) { health -= amount; UpdateUI(); }

    void UpdateUI()
    {
        gemText.text = "Gems: " + score;
        healthText.text = "HP: " + health;
    }

    void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

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