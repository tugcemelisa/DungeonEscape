using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [Header("Sınır Ayarları")]
    public float minZ = 0.368f; // Başlangıç noktası
    public float maxZ = 2.138f; // Varış noktası
    public float speed = 2.0f;  // Hareket hızı

    void Update()
    {
        // 1. İki nokta arasındaki toplam mesafeyi hesapla
        float duration = maxZ - minZ;

        // 2. PingPong ile 0 ile mesafe arasında bir değer üret
        float offset = Mathf.PingPong(Time.time * speed, duration);

        // 3. Testerenin pozisyonunu güncelle (X ve Y sabit, Z değişken)
        transform.position = new Vector3(transform.position.x, transform.position.y, minZ + offset);
    }
}