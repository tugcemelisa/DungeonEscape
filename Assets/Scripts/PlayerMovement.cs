using UnityEngine;
using TMPro; // KRİTİK: Bu satır, kodun TextMeshPro ile konuşmasını sağlar

public class PlayerMovement : MonoBehaviour
{
    // --- 1. REFERENCES ---
    [Header("UI Settings")]
    public TextMeshProUGUI gemText; // Ekrandaki yazı objesini buraya bağlayacağız

    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 5.0f;

    [Header("Game Data")]
    public int score = 0;

    private Rigidbody rb;

    // --- 2. INITIALIZATION ---
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateUI(); // Oyun başlar başlamaz ekranı güncelle
    }

    // --- 3. LOOP ---
    void Update()
    {
        // Sprint System
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = speed * 2;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.Translate(x * currentSpeed * Time.deltaTime, 0, z * currentSpeed * Time.deltaTime);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // --- 4. INTERACTION ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gem"))
        {
            score += 10;
            UpdateUI(); // Mücevher toplandığında yazıyı güncelle
            Debug.Log("Gem Collected! Current Score: " + score);
            Destroy(other.gameObject);
        }
    }

    // --- 5. UI UPDATE METHOD ---
    void UpdateUI()
    {
        // Ekrandaki yazıya "Gems: 10" gibi değerleri gönderir
        gemText.text = "Gems: " + score;
    }
}