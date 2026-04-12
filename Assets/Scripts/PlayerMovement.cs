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
    public float jumpForce = 12.0f; // Jerry'nin kütlesine göre 12 idealdir
    private Rigidbody rb;
    private Animator anim;

    [Header("Ground Check (Physics Detection)")]
    public Transform groundCheck;
    public float groundDistance = 0.5f; // Merdivenler için 0.5 daha güvenlidir
    public LayerMask groundMask;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Jerry'nin altındaki Animator'ü bulur
        anim = GetComponentInChildren<Animator>();
        UpdateUI();
        ShowInfo("Explore the dungeon and find the exit!");
    }

    void Update()
    {
        // YER KONTROLÜ: Radar sistemi
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Send isGrounded to Animator (optional parameter used for transitions)
        if (anim != null)
        {
            anim.SetBool("isGrounded", isGrounded);
        }

        HandleMovement();
        if (health <= 0) RestartGame();
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * z) + (camRight * x);
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2 : speed;

        // 1. BLEND TREE TETIKLEME: Hız bilgisini Animator'e gönderir
        float velocity = moveDirection.magnitude * currentSpeed;
        if (anim != null)
        {
            anim.SetFloat("MoveSpeed", velocity);
        }

        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        // 2. ZIPLAMA VE ANIMASYON TETIKLEME
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Trigger Jump animation first (animation anticipation)
            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }

            // Reset vertical velocity before applying jump force
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }

            // Cancel any pending invokes and apply jump physics after a short delay so animation can play anticipation
            CancelInvoke(nameof(ApplyJumpForce));
            Invoke(nameof(ApplyJumpForce), 0.1f);
        }
    }

    // Apply the physical jump force after a short delay so the animation can play its anticipation.
    void ApplyJumpForce()
    {
        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // --- TETIKLEYICILER (Key, Enemy, Gem) ---
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("IronDoor"))
        {
            if (hasKey)
            {
                ShowInfo("The Door is opening...");
                Transform doorTrans = collision.transform;
                doorTrans.rotation = Quaternion.Euler(doorTrans.eulerAngles.x, doorTrans.eulerAngles.y, -90f);
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