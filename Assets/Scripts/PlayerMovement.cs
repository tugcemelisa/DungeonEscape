using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("VFX")]
    public ParticleSystem gemPickupFX;
    public ParticleSystem doorOpenFX;
    public ParticleSystem deathFX;
    public ParticleSystem damageFX;

    [Header("Damage Flash")]
    public Image damageFlash;

    [Header("Player Stats")]
    public int score = 0;
    public int health = 100;
    public int maxHealth = 100;

    [Header("Movement Settings")]
    public float speed = 2.5f;
    public float jumpForce = 6.0f;

    private Rigidbody rb;
    private Animator anim;
    private bool _isDead;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;
    private bool isGrounded;

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
        if (_isDead) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        ReadInput();

        if (anim != null)
        {
            float currentSpeed = _isSprinting ? speed * 2f : speed;
            anim.SetFloat("MoveSpeed", _moveDir.magnitude * currentSpeed, 0.08f, Time.deltaTime);
        }

        if (health <= 0) Die();
    }

    void ReadInput()
    {
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
        if (_isDead) return;

        float currentSpeed = _isSprinting ? speed * 2f : speed;

        if (_moveDir != Vector3.zero)
        {
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
        if (_isDead) return;

        if (other.CompareTag("Key") && !hasKey)
        {
            hasKey = true;
            heldKey = other.gameObject;
            heldKey.transform.SetParent(playerHand);
            heldKey.transform.localPosition = Vector3.zero;
            heldKey.transform.localRotation = Quaternion.identity;
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

            if (gem != null && gem.pickupSound != null)
                AudioSource.PlayClipAtPoint(gem.pickupSound, other.transform.position);

            // Spawn gem particle before destroying the gem
            if (gem != null) SpawnFX(gem.collectFX, other.transform.position);
            else             SpawnFX(gemPickupFX,    other.transform.position);

            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDead) return;

        if (collision.gameObject.CompareTag("IronDoor"))
        {
            if (hasKey)
            {
                ShowInfo("The Door is opening...");
                Transform doorTrans = collision.transform;
                doorTrans.rotation = Quaternion.Euler(doorTrans.eulerAngles.x, doorTrans.eulerAngles.y, -90f);
                collision.collider.isTrigger = true;

                heldKey.transform.SetParent(doorTrans);
                heldKey.transform.localPosition = new Vector3(-27.7900009f, -2.71000004f, 1.71000004f);
                heldKey.transform.localRotation = Quaternion.Euler(0f, 0f, 90.9999924f);
                heldKey.transform.localScale    = new Vector3(1.14138663f, 1.12065291f, 1.95453012f);

                if (doorOpenSound != null)
                    AudioSource.PlayClipAtPoint(doorOpenSound, doorTrans.position);

                SpawnFX(doorOpenFX, doorTrans.position);

                heldKey = null;
                hasKey = false;

                // Level exit door kontrolü
                var ld = collision.gameObject.GetComponent<LevelDoor>();
                if (ld != null)
                    GameManager.Instance?.TriggerLevelComplete(score, ld.nextSceneName);
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
        SpawnFX(damageFX, transform.position);
        if (damageFlash != null) StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        damageFlash.color = new Color(1f, 0f, 0f, 0.45f);
        float duration = 0.35f;
        float elapsed  = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.45f, 0f, elapsed / duration);
            damageFlash.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
        damageFlash.color = new Color(1f, 0f, 0f, 0f);
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        UpdateUI();
        ShowInfo($"+{amount} HP restored!");
    }

    void Die()
    {
        _isDead = true;
        SpawnFX(deathFX, transform.position);

        if (GameManager.Instance != null)
            GameManager.Instance.TriggerLose(score);
        else
            Invoke(nameof(FallbackRestart), 1.5f);
    }

    void FallbackRestart() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    // --- VFX HELPER ---

    void SpawnFX(ParticleSystem prefab, Vector3 pos)
    {
        if (prefab == null) return;
        var fx = Instantiate(prefab, pos, Quaternion.identity);
        Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax + 1f);
    }

    // --- UI HELPERS ---

    void ShowInfo(string message)
    {
        if (infoText == null) return;
        infoText.text = message;
        CancelInvoke(nameof(ClearText));
        Invoke(nameof(ClearText), 3.0f);
    }

    void ClearText() { if (infoText != null) infoText.text = ""; }

    void UpdateUI()
    {
        if (gemText)    gemText.text    = "Gems: " + score;
        if (healthText) healthText.text = "HP: "   + health;
    }
}
