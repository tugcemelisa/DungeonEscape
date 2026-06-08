using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [Header("Potion Settings")]
    public int healAmount = 30;

    [Header("SFX")]
    public AudioClip pickupSound;

    [Header("VFX")]
    public ParticleSystem healFX;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;

        player.Heal(healAmount);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        SpawnFX(healFX, transform.position);
        Destroy(gameObject);
    }

    void SpawnFX(ParticleSystem prefab, Vector3 pos)
    {
        if (prefab == null) return;
        var fx = Instantiate(prefab, pos, Quaternion.identity);
        Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax + 1f);
    }
}
