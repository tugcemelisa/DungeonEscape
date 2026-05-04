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

    [Header("SFX")]
    public AudioClip keyPickupSound;
    public AudioClip doorOpenSound;

    [Header("Player Stats")]
    public int score = 0;
    public int health = 100;
    public int maxHealth = 100;

    [Header("Movement Settings")]
    public float speed = 2.5f;

    public float jumpForce = 6.0f;
    private Rigidbody rb;
    private Animator anim;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;
    private bool isGrounded;

    // Physics input cache — written in Update, consumed in FixedUpdate
    private Vector3 _moveDir = Vector3.zero;
    private bool _isSprinting = false;
    private bool _jumpQueued = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        UpdateUI();
        ShowInfo("Explore the dungeon and find the exit!");
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        ReadInput();

        // Update'te SetFloat -> her frame blend guncellenir -> smooth animasyon
        if (anim != null)
        {
            float currentSpeed = _isSprinting ? speed * 2f : speed;
            anim.SetFloat("MoveSpeed", _moveDir.magnitude * currentSpeed, 0.08f, Time.deltaTime);
        }

        if (health <= 0) RestartGame();
    }

    void ReadInput()
    {
        // GetAxisRaw: tuş bırakılınca anında 0 — smooth decay yok, idle kayma yok
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight   = Camera.main.transform.right;
        camForward.y = 0; camForward.Normalize();
        camRight.y   = 0; camRight.Normalize();

        _moveDir = (camForward * z) + (camRight * x);
        if (_moveDir.magnitude > 1f) _moveDir.Normalize();

        _isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            _jumpQueued = true;
    }

    void FixedUpdate()
    {
        float currentSpeed = _isSprinting ? speed * 2f : speed;

        if (_moveDir != Vector3.zero)
        {
            // linearVelocity → Rigidbody interpolation ile cok daha smooth calisir
            Vector3 targetVel = _moveDir * currentSpeed;
            rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
            Quaternion targetRot = Quaternion.LookRotation(_moveDir);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, 600f * Time.fixedDeltaTime));
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        if (_jumpQueued)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (anim != null) anim.SetTrigger("Jump");
            _jumpQueued = false;
        }
    }

    // --- TRIGGERS ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") && !hasKey)
        {
            hasKey = true;
            heldKey = other.gameObject;
            heldKey.transform.SetParent(playerHand);
            heldKey.transform.localPosition = new Vector3(0f, 0f, 0f);
            heldKey.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            heldKey.GetComponent<Collider>().enabled = false;
            if (keyPickupSound != null)
                AudioSource.PlayClipAtPoint(keyPickupSound, other.transform.position);
            ShowInfo("Key Collected! Find the Iron Door.");
        }

        if (other.CompareTag("Enemy"))
        {
            TakeDamage(20);
            ShowInfo("Ouch! Be careful!");
        }

        if (other.CompareTag("Gem"))
        {
            GemData gem = other.GetComponent<GemData>();
            int points = gem != null ? gem.gemValue : 10;
            score += points;
            UpdateUI();

            // SFX — plays at world position even after object is destroyed
            if (gem != null && gem.pickupSound != null)
                AudioSource.PlayClipAtPoint(gem.pickupSound, other.transform.position);

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

                heldKey.transform.SetParent(doorTrans);
                heldKey.transform.localPosition = new Vector3(-27.7900009f, -2.71000004f,  1.71000004f);
                heldKey.transform.localRotation = Quaternion.Euler(0f, 0f, 90.9999924f);
                heldKey.transform.localScale    = new Vector3(1.14138663f,  1.12065291f,  1.95453012f);
                if (doorOpenSound != null)
                    AudioSource.PlayClipAtPoint(doorOpenSound, doorTrans.position);
                heldKey = null;
                hasKey = false;
            }
            else
            {
                ShowInfo("Locked! You need a Golden Key.");
            }
        }
    }

    // --- HEALTH ---

    void TakeDamage(int amount)
    {
        health -= amount;
        UpdateUI();
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        UpdateUI();
        ShowInfo($"+{amount} HP restored!");
    }

    // --- UI HELPERS ---

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
        gemText.text    = "Gems: " + score;
        healthText.text = "HP: "   + health;
    }

    void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
}
