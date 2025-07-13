using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public DartThrowController dartThrowController;
    public MonsterManager monsterManager;

    public Text scoreText;
    public Text liveScoreText;
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

        if (levelCleared) return;

        if (dartThrowController.throwsThisRound >= dartThrowController.maxThrowsPerRound)
        {
            roundActive = false;
            StartCoroutine(MonsterTurnAndFinalCheck());
        }
    }

    IEnumerator MonsterTurnAndFinalCheck()
    {
        // Wait a moment for possible late collision detection
        yield return new WaitForSeconds(0.3f);

        if (monsterManager.AreAllMonstersDefeated())
        {
            OnLevelCleared();
            yield break;
        }

        if (monsterManager != null)
            monsterManager.MoveOrAttackMonsters();

        yield return new WaitForSeconds(1f);

        // Double-check again after monster animation/move
        if (levelCleared) yield break;

        if (monsterManager.AreAllMonstersDefeated())
        {
            OnLevelCleared();
        }
        else
        {
            GameOver();
        }
    }

    void StartLevel()
    {
        levelCleared = false;

        levelText.text = $"{level}";
        dartThrowController.ResetThrows();
        monsterManager.SpawnMonsters(level);
        dartThrowController.StartNewRound();

        roundActive = true;
        UpdateUI();
    }

    void OnLevelCleared()
    {
        if (levelCleared) return;

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
        liveScoreText.text = $"{CalculateScore()}";
    }

    public void GameOver()
    {
        if (levelCleared) return; // Don't show game over if already cleared
        roundActive = false;
        gameOverPanel.SetActive(true);
        scoreText.text = $"Score: {CalculateScore()}";
    }

    int CalculateScore()
    {
        return Mathf.Max(0, (int)(1000 - (elapsedTime * 10 + totalThrows * 5)));
    }

    public void ReplayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BullseyeBill");
    }
}
