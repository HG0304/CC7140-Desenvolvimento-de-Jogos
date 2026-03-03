using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public int lives = 3;
    public bool gameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance?.UpdateScore(score);
    }

    public void LoseLife()
    {
        if (gameOver) return;
        lives--;
        UIManager.Instance?.UpdateLives(lives);
        if (lives <= 0)
            TriggerDefeat();
        else
            FindFirstObjectByType<Player>()?.Respawn();
    }

    public void TriggerVictory()
    {
        if (gameOver) return;
        gameOver = true;
        Invoke(nameof(LoadVictory), 1.5f);
    }

    public void TriggerDefeat()
    {
        if (gameOver) return;
        gameOver = true;
        Invoke(nameof(LoadDefeat), 1.5f);
    }

    private void LoadVictory() => SceneManager.LoadScene("Victory");
    private void LoadDefeat() => SceneManager.LoadScene("Defeat");

    public void RestartGame()
    {
        score = 0;
        lives = 3;
        gameOver = false;
        SceneManager.LoadScene("SampleScene");
    }
}
