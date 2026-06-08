using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.Restart();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
