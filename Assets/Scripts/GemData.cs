using UnityEngine;

public class GemData : MonoBehaviour
{
    [Header("Gem Properties")]
    public int gemValue = 10;

    [Header("SFX")]
    public AudioClip pickupSound;

    [Header("Visual")]
    public float spinSpeed = 80f;
    public float bobSpeed = 1.5f;
    public float bobHeight = 0.12f;

    private float _startY;

    void Start()
    {
        // Gem'i zemin yuzeyine yasla -- trigger collider'lari yoksay
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 5f,
                Physics.AllLayers, QueryTriggerInteraction.Ignore))
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);

        _startY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        float newY = _startY + (Mathf.Sin(Time.time * bobSpeed) + 1f) * 0.5f * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}