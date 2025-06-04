using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text throwCountText;
    public TMP_Text levelText;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    private float timeElapsed = 0f;
    private int totalThrows = 0;
    private bool isGameActive = true;

    void Update()
    {
        if (!isGameActive) return;

        timeElapsed += Time.deltaTime;
        UpdateTimerDisplay();
    }

    public void UpdateThrowCount(int throwsRemaining)
    {
        totalThrows++;
        if (throwCountText != null)
            throwCountText.text = $"Throws Left: {throwsRemaining}";
    }

    public void ShowLevelUp(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
            StartCoroutine(FlashLevelText());
        }
    }

    private IEnumerator FlashLevelText()
    {
        levelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        levelText.gameObject.SetActive(false);
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
            timerText.text = $"Time: {timeElapsed:F1}s";
    }

    public void ShowGameOver()
    {
        isGameActive = false;

        float finalScore = CalculateScore();
        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {finalScore:F0}\nTime: {timeElapsed:F1}s\nThrows: {totalThrows}";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private float CalculateScore()
    {
        // Basic example: higher score = faster + fewer throws
        return Mathf.Max(0, 10000 - (timeElapsed * 10f + totalThrows * 50f));
    }

    public void ResetUI()
    {
        timeElapsed = 0f;
        totalThrows = 0;
        isGameActive = true;

        if (throwCountText != null) throwCountText.text = "Throws Left: 3";
        if (timerText != null) timerText.text = "Time: 0.0s";
        if (scoreText != null) scoreText.text = "Score: 0";
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}"; 
    }
}
