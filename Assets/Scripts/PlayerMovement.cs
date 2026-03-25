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
    public Transform playerHand;
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

    void HandleMovement()
    {
        // 1. KLAVYEDEN GİRİŞLERİ AL
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 2. KAMERANIN BAKTIĞI YÖNLERİ REFERANS AL (DERS 10 GÜNCELLEMESİ)
        // Kameranın 'forward' ve 'right' yönlerini alıyoruz
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Karakterin yere gömülmemesi için Y eksenini sıfırlıyoruz
        camForward.y = 0;
        camRight.y = 0;

        // Vektörleri 'normalize' ederek hızı sabitliyoruz
        camForward.Normalize();
        camRight.Normalize();

        // 3. YENİ HAREKET YÖNÜNÜ HESAPLA
        Vector3 moveDirection = (camForward * z) + (camRight * x);

        // Koşma kontrolü (Shift)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2 : speed;

        // 4. KARAKTERİ DÜNYA EKSENİNDE HAREKET ETTİR (Space.World kritik!)
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

        // 5. GÖRSEL DÜZELTME: Karakter gittiği yöne doğru anında dönsün
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        // ZIPLAMA (Fizik tabanlı olduğu için aynı kalıyor)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // --- TRIGGER EVENTS (Gem, Enemy, Key) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key") && !hasKey)
        {
            hasKey = true;
            heldKey = other.gameObject;
            heldKey.transform.SetParent(playerHand);
            heldKey.transform.localPosition = Vector3.zero;
            heldKey.transform.localRotation = Quaternion.Euler(0, 90f, 0);
            heldKey.GetComponent<Collider>().enabled = false;
            ShowInfo("Key Collected! Find the Iron Door.");
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20);
            ShowInfo("Ouch! Be careful!");
        }

        if (other.gameObject.CompareTag("Gem"))
        {
            score += 10;
            UpdateUI();
            Destroy(other.gameObject);
        }
    }

    // --- COLLISION EVENTS (Iron Door) ---
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("IronDoor"))
        {
            if (hasKey)
            {
                ShowInfo("The Door is opening...");
                Transform doorTrans = collision.transform;
                float currentX = doorTrans.eulerAngles.x;
                float currentY = doorTrans.eulerAngles.y;
                doorTrans.rotation = Quaternion.Euler(currentX, currentY, -90f);

                collision.collider.isTrigger = true;
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
    void TakeDamage(int amount)
    {
        health -= amount;
        UpdateUI();
    }

    void ShowInfo(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
            CancelInvoke("ClearText");
            Invoke("ClearText", 3.0f);
        }
    }

    void ClearText() { if (infoText != null) infoText.text = ""; }

    void UpdateUI()
    {
        gemText.text = "Gems: " + score;
        healthText.text = "HP: " + health;
    }

    void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
}