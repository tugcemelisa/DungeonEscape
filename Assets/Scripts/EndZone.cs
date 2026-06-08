using UnityEngine;

public class EndZone : MonoBehaviour
{
    [Header("VFX")]
    public ParticleSystem winFX;

    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player")) return;
        _triggered = true;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        SpawnFX(winFX, transform.position);

        if (GameManager.Instance != null)
            GameManager.Instance.TriggerWin(player != null ? player.score : 0);
    }

    void SpawnFX(ParticleSystem prefab, Vector3 pos)
    {
        if (prefab == null) return;
        var fx = Instantiate(prefab, pos, Quaternion.identity);
        Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax + 1f);
    }
}
