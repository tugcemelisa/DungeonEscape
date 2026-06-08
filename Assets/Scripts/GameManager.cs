using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Won, Dead }
    public GameState State { get; private set; } = GameState.Playing;

    [Header("Win Panel")]
    public GameObject winPanel;
    public TextMeshProUGUI winScoreText;
    public TextMeshProUGUI winTimeText;

    [Header("Lose Panel")]
    public GameObject losePanel;
    public TextMeshProUGUI loseScoreText;

    private float _startTime;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _startTime = Time.time;
        if (winPanel)  winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    public void TriggerWin(int score)
    {
        if (State != GameState.Playing) return;
        State = GameState.Won;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        float t = Time.time - _startTime;
        if (winScoreText) winScoreText.text  = "Score: " + score;
        if (winTimeText)  winTimeText.text   = $"Time: {(int)t / 60:00}:{(int)t % 60:00}";
        if (winPanel)     winPanel.SetActive(true);
    }

    public void TriggerLose(int score)
    {
        if (State != GameState.Playing) return;
        State = GameState.Dead;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (loseScoreText) loseScoreText.text = "Score: " + score;
        if (losePanel)     losePanel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
