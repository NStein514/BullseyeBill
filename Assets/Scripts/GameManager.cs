using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    void Start()
    {
        gameOverPanel.SetActive(false);
        dartThrowController.OnDartThrown += OnDartThrown;
        monsterManager.OnMonstersDefeated += OnLevelCleared;
        UpdateUI();
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

        monsterManager.MoveMonsters();

        yield return new WaitForSeconds(1f);

        monsterManager.CheckMonstersDefeated();

        if (roundActive == false)
        {
            StartNewRound();
        }
    }

    void StartNewRound()
    {
        dartThrowController.ResetThrows();
        roundActive = true;
        UpdateUI();
    }

    void OnLevelCleared()
    {
        level++;
        levelText.text = "Level: " + level;

        // TODO: Load new monster pattern for next level

        StartNewRound();
    }

    void UpdateUI()
    {
        throwCountText.text = $"Throws: {totalThrows}";
        levelText.text = "Level: " + level;
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
