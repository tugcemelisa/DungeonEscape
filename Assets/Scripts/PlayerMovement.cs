using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    // 1. Değişkenler (Variables): Kahramanın ayarlanabilir özellikleri
    public float speed = 5.0f; // Hareket hızı
    void Update()
    {
        // 2. Klavyeden komutları yakala (W,S,A,D tuşları)
        float x = Input.GetAxis("Horizontal"); // A-D (Sağa-Sola)
        float z = Input.GetAxis("Vertical");   // W-S (İleri-Geri)
        // 3. Karakteri hareket ettir
        transform.Translate(x * speed * Time.deltaTime, 0, z * speed * Time.deltaTime);
    }
}
