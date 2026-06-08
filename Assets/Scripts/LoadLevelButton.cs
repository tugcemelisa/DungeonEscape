using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    public string sceneName = "Level2";

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
        SceneManager.LoadScene(sceneName);
    }
}
