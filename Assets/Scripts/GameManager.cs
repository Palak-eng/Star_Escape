using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject pausePanel;

    [Header("Game Settings")]
    public int targetScore = 10;

    private int score = 0;
    private int highScore = 0;

    private bool gameOver = false;
    private bool isPaused = false;
    private bool hasStarted = false;

    public bool IsGameOver => gameOver;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 🔁 Check if game restarted
        int restarted = PlayerPrefs.GetInt("Restarted", 0);

        if (restarted == 1)
        {
            hasStarted = true;
            Time.timeScale = 1f;

            if (startPanel != null)
                startPanel.SetActive(false);

            PlayerPrefs.SetInt("Restarted", 0); // reset
        }
        else
        {
            Time.timeScale = 0f;

            if (startPanel != null)
                startPanel.SetActive(true);
        }

        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateScoreUI();
        UpdateHighScoreUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (!hasStarted || gameOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // ▶️ START GAME
    public void StartGame()
    {
        hasStarted = true;
        isPaused = false;
        gameOver = false;

        if (startPanel != null)
            startPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // ⭐ ADD SCORE
    public void AddScore(int amount)
    {
        if (gameOver || !hasStarted)
            return;

        score += amount;
        UpdateScoreUI();

        // 📊 High Score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }

        // 🏆 Win Condition
        if (score >= targetScore)
        {
            WinGame();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = "Best: " + highScore;
    }

    // 💀 GAME OVER
    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;
        isPaused = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // 🏆 WIN
    public void WinGame()
    {
        if (gameOver)
            return;

        gameOver = true;
        isPaused = false;

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // ⏸️ PAUSE
    public void TogglePause()
    {
        if (gameOver || !hasStarted)
            return;

        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        if (gameOver || !hasStarted)
            return;

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // 🔁 RESTART (skip start panel)
    public void RestartGame()
    {
        PlayerPrefs.SetInt("Restarted", 1);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ❌ QUIT
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}