using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    // --- 1. VARIABLES ---
    [Header("Patrol Settings")]
    public Transform pointA;    // Starting point
    public Transform pointB;    // Ending point
    public float speed = 2.0f;  // Enemy's movement speed

    private Transform targetPoint; // Current destination

    // --- 2. INITIALIZATION ---
    void Start()
    {
        // Start by heading towards Point B
        targetPoint = pointB;
    }

    // --- 3. LOOP ---
    void Update()
    {
        // Move the enemy towards the targetPoint
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Check if the enemy has reached the target
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Switch targets: If at A go to B, if at B go to A
            if (targetPoint == pointB)
                targetPoint = pointA;
            else
                targetPoint = pointA; // Small error check: switch logic
            targetPoint = (targetPoint == pointB) ? pointA : pointB;
        }
    }
}