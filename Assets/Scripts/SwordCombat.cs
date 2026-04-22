using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackDamage   = 30f;
    public float attackRadius   = 0.65f;
    public float attackCooldown = 0.7f;

    [Header("Weapon Mount")]
    public GameObject swordPrefab;
    public Transform  swordSocket;
    public Vector3 swordLocalPosition = Vector3.zero;
    public Vector3 swordLocalEuler    = Vector3.zero;
    public Vector3 swordLocalScale    = new Vector3(0.35f, 0.35f, 0.35f);

    private Animator anim;
    private Transform swordTransform;
    private bool  isAttacking;
    private float nextAttackTime;
    private readonly HashSet<EnemyHealth> hitThisSwing = new HashSet<EnemyHealth>();
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    // Mid-blade point in sword local space
    private static readonly Vector3 BladeLocalMid = new Vector3(0f, 1.2f, 0f);

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        SpawnSword();
    }

    void SpawnSword()
    {
        if (swordPrefab == null) { Debug.LogWarning("[SwordCombat] swordPrefab is null!"); return; }

        Transform mount = swordSocket;
        if (mount == null)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            mount = (pm != null && pm.playerHand != null) ? pm.playerHand : transform;
        }

        GameObject go = Instantiate(swordPrefab, mount);
        go.transform.localPosition = swordLocalPosition;
        go.transform.localRotation = Quaternion.Euler(swordLocalEuler);
        go.transform.localScale    = swordLocalScale;
        swordTransform = go.transform;

        Debug.Log($"[SwordCombat] Sword spawned under '{mount.name}' (instanceID {mount.GetInstanceID()})");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        hitThisSwing.Clear();

        // Fire attack animation — sword moves ONLY because the hand bone moves
        if (anim != null)
        {
            anim.SetTrigger(AttackHash);
            Debug.Log("[SwordCombat] Attack trigger sent to Animator.");
        }

        yield return new WaitForSeconds(0.15f);

        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            ScanForHits();
            elapsed += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    void ScanForHits()
    {
        if (swordTransform == null) return;
        Vector3 bladeWorld = swordTransform.TransformPoint(BladeLocalMid);
        Collider[] hits = Physics.OverlapSphere(bladeWorld, attackRadius);
        foreach (Collider col in hits)
        {
            if (col.transform.IsChildOf(transform)) continue;
            EnemyHealth enemy = col.GetComponentInParent<EnemyHealth>();
            if (enemy != null && hitThisSwing.Add(enemy))
            {
                enemy.TakeDamage((int)attackDamage);
                Debug.Log($"[SwordCombat] Hit '{enemy.name}' for {attackDamage} damage.");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || swordTransform == null) return;
        Vector3 bladeWorld = swordTransform.TransformPoint(BladeLocalMid);
        Gizmos.color = isAttacking ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(bladeWorld, attackRadius);
    }
}
