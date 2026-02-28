using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints; // Array to store our 4 waypoints
    public float speed = 3.0f;

    private int currentPointIndex = 0; // Tracking which point to visit next

    void Update()
    {
        // Move towards the current target waypoint
        Transform target = waypoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Check if the enemy is close enough to the waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            // Cycle through points: 0, 1, 2, 3, then back to 0
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }
    }
}