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
    public float speed = 2.5f;
    public float jumpForce = 6.0f;
    private Rigidbody rb;
    private Animator anim; // Animasyonları tetiklemek için

    [Header("Ground Check (New!)")]
    public Transform groundCheck;    // Karakterin altına koyacağımız boş obje
    public float groundDistance = 0.3f; // Yere ne kadar yakın olmalı?
    public LayerMask groundMask;      // Hangi katmanı 'yer' olarak görecek?
    private bool isGrounded;         // Yerde miyiz?

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); // Jerry üzerindeki Animator'ü bulur
        UpdateUI();
        ShowInfo("Explore the dungeon and find the exit!");
    }

    void Update()
    {
        // YER KONTROLÜ: Her karede yere değip değmediğimizi kontrol eder
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

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

        // ANIMASYON TETIKLEME: MoveSpeed parametresini günceller
        float velocity = moveDirection.magnitude * currentSpeed;
        if (anim != null) anim.SetFloat("MoveSpeed", velocity);

        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        // ZIPLAMA GÜNCELLEMESI: Sadece 'isGrounded' true ise zıplar
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (anim != null) anim.SetTrigger("Jump");
        }
    }

    // --- TETIKLEYICILER VE DIGERLERI (AYNEN KALDI) ---
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