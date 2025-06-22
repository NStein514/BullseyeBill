using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.IMGUI.Controls;

public class GameManager : MonoBehaviour
{
    public DartThrowController dartThrowController;
    public MonsterManager monsterManager;

    public Text scoreText;
    public Text throwCountText;
    public Text timerText;
    public Text levelText;

    public GameObject gameOverPanel;

    private int level = 1;
    private int totalThrows = 0;
    private float elapsedTime = 0f;

    private bool roundActive = true;
    private bool levelCleared = false;

    void Start()
    {
        gameOverPanel.SetActive(false);

        if (dartThrowController != null)
            dartThrowController.OnDartThrown += OnDartThrown;

        if (monsterManager != null)
            monsterManager.OnAllMonstersDefeated += OnLevelCleared;

        StartLevel();
    }

    void Update()
    {
        if (roundActive)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = "Time: " + elapsedTime.ToString("F1") + "s";
        }
    }

    void OnDartThrown()
    {
        totalThrows++;
        throwCountText.text = $"Throws: {totalThrows}";

        if (dartThrowController != null)
        {
            if (dartThrowController.throwsThisRound >= dartThrowController.maxThrowsPerRound)
            {
                roundActive = false;

                // After delay, move monsters and start new round
                StartCoroutine(MonsterTurnAndNextRound());
            }
        }
    }

    IEnumerator MonsterTurnAndNextRound()
    {
        yield return new WaitForSeconds(1f);

        if (monsterManager != null)
            monsterManager.MoveOrAttackMonsters(); 

        yield return new WaitForSeconds(1f);

        if (!levelCleared)
        {
            StartNewRound();
        }
    }

    void StartLevel()
    {
        levelCleared = false;
        levelText.text = $"{level}";

        if (monsterManager != null)
            monsterManager.SpawnMonsters(level);

        StartNewRound(); 
    }

    void StartNewRound()
    {
        dartThrowController.ResetThrows();
        roundActive = true;
        UpdateUI(); 
    }

    void OnLevelCleared()
    {
        levelCleared = true; 
        level++;

        StartCoroutine(AdvanceToNextLevel()); 
    }

    IEnumerator AdvanceToNextLevel()
    {
        roundActive = false;
        yield return new WaitForSeconds(1f);
        StartLevel(); 
    }

    void UpdateUI()
    {
        throwCountText.text = $"Throws: {totalThrows}";
        levelText.text = $"{level}";
    }

    public void GameOver()
    {
        roundActive = false;
        gameOverPanel.SetActive(true);
        scoreText.text = $"Score: {CalculateScore()}";
    }

    int CalculateScore()
    {
        // Example: score based on time & throws, lower is better
        int score = Mathf.Max(0, (int)(1000 - (elapsedTime * 10 + totalThrows * 5)));
        return score;
    }
}
