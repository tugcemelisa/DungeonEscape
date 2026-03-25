using UnityEngine;
public class Pendulum : MonoBehaviour
{
    public float angle = 45f; public float speed = 2f;
    private Quaternion startRotation;
    void Start() { startRotation = transform.rotation; }
    void Update()
    {
        float angleShift = Mathf.Sin(Time.time * speed) * angle;
        transform.rotation = startRotation * Quaternion.Euler(0, 0, angleShift);
    }
}