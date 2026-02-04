using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //1. DEĞİŞKENLER (Karakter Özellikleri) 
    [Header("Hareket Ayarları")]
    public float speed = 5.0f;     // Yürüme hızı
    public float jumpForce = 5.0f;  // Zıplama gücü

    [Header("Oyun Verileri")]
    public int score = 0;          // Toplanan altın sayısı

    private Rigidbody rb;          // Fizik motoru referansı

    //2. HAZIRLIK (Oyun Başladığında) 
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Fizik motorunu bağla
    }

    //3. DÖNGÜ (Her Karede Kontrol Et) 
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
   

        transform.Translate(x * speed * Time.deltaTime, 0, z * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    //4. ETKİLEŞİM (Temas Anında)
    private void OnTriggerEnter(Collider other)
    {
        // Eğer çarptığımız objenin yakasında "Gold" etiketi varsa
        if (other.gameObject.CompareTag("Gold"))
        {
            score += 10; // Skor kutusuna 10 ekle
            Debug.Log("Altın Toplandı! Mevcut Skor: " + score);

            // Altını dünyadan sil (Artık karakterin çantasında)
            Destroy(other.gameObject);
        }
    }
}