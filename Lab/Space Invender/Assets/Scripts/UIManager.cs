using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI livesText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void SetTexts(TextMeshProUGUI score, TextMeshProUGUI lives)
    {
        scoreText = score;
        livesText = lives;
        if (GameManager.Instance != null)
        {
            UpdateScore(GameManager.Instance.score);
            UpdateLives(GameManager.Instance.lives);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = "SCORE: " + score.ToString("D6");
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null) livesText.text = "LIVES: " + lives;
    }
}
