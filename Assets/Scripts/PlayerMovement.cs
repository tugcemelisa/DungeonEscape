using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Required to restart the level

public class PlayerMovement : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI healthText;

    [Header("Player Stats")]
    public int score = 0;
    public int health = 100; // Starting health value

    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 5.0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateUI();
    }

    void Update()
    {
        HandleMovement();

        // Check if player is dead
        if (health <= 0)
        {
            RestartGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle Gem Collection (Multi-colored)
        if (other.gameObject.CompareTag("Gem"))
        {
            GemData data = other.GetComponent<GemData>();
            if (data != null)
            {
                score += data.gemValue;
                UpdateUI();
            }
            Destroy(other.gameObject);
        }

        // Handle Enemy Collision (Damage)
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20); // Lose 20 HP when touching enemy
        }
    }

    // Helper method to reduce health
    void TakeDamage(int amount)
    {
        health -= amount;
        UpdateUI();
        Debug.Log("Ouch! Health left: " + health);
    }

    void UpdateUI()
    {
        gemText.text = "Gems: " + score;
        healthText.text = "HP: " + health;
    }

    void RestartGame()
    {
        // Reload the current active scene from the beginning
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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