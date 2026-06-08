using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Won, Dead, LevelComplete }
    public GameState State { get; private set; } = GameState.Playing;

    [Header("HUD")]
    public GameObject hudPanel;

    [Header("Win Panel")]
    public GameObject winPanel;
    public TextMeshProUGUI winScoreText;
    public TextMeshProUGUI winTimeText;

    [Header("Lose Panel")]
    public GameObject losePanel;
    public TextMeshProUGUI loseScoreText;

    [Header("Level Complete Panel")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteScoreText;

    private float _startTime;
    private string _nextScene = "Level2";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _startTime = Time.time;
        if (winPanel)            winPanel.SetActive(false);
        if (losePanel)           losePanel.SetActive(false);
        if (levelCompletePanel)  levelCompletePanel.SetActive(false);
    }

    public void TriggerWin(int score)
    {
        if (State != GameState.Playing) return;
        State = GameState.Won;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (hudPanel) hudPanel.SetActive(false);

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

        if (hudPanel) hudPanel.SetActive(false);

        if (loseScoreText) loseScoreText.text = "Score: " + score;
        if (losePanel)     losePanel.SetActive(true);
    }

    public void TriggerLevelComplete(int score, string nextScene = "Level2")
    {
        if (State != GameState.Playing) return;
        State = GameState.LevelComplete;
        Time.timeScale = 0f;

        _nextScene = nextScene;
        GameProgress.CarryScore  = score;
        GameProgress.CurrentLevel = 2;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (hudPanel) hudPanel.SetActive(false);
        if (levelCompleteScoreText) levelCompleteScoreText.text = "Score: " + score;
        if (levelCompletePanel)     levelCompletePanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_nextScene);
    }

    public void Restart()
    {
        GameProgress.Reset();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
