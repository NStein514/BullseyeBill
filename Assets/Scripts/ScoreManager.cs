using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int totalScore = 0;
    private int totalDamageDealt = 0;
    private int totalThrows = 0;

    [Header("Optional UI Integration")]
    public UIManager uiManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int baseScore, int multiplier)
    {
        int scoreThisHit = baseScore * multiplier;
        totalScore += scoreThisHit;
        totalDamageDealt += scoreThisHit;
        totalThrows++;

        uiManager?.UpdateScoreDisplay(totalScore);
    }

    public int GetTotalScore() => totalScore;
    public int GetTotalDamage() => totalDamageDealt;
    public int GetTotalThrows() => totalThrows;

    public void ResetScore()
    {
        totalScore = 0;
        totalDamageDealt = 0;
        totalThrows = 0;

        uiManager?.UpdateScoreDisplay(totalScore); 
    }
}
