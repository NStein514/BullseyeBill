using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int throwsPerRound = 3;
    public float delayBeforeMonstersMove = 1.5f;
    public float delayBeforeNextRound = 2f;

    [Header("References")]
    public DartThrowController dartThrowController;
    public MonsterManager monsterManager;
    public UIManager uiManager;

    private int currentLevel = 1;
    private int throwsUsed = 0;
    private bool roundInProgress = true;

    void Start()
    {
        StartRound();
    }

    public void OnDartThrown(int throwsUsedSoFar)
    {
        throwsUsed = throwsUsedSoFar;
        uiManager?.UpdateThrowCount(throwsPerRound - throwsUsed);

        if (throwsUsed >= throwsPerRound)
        {
            StartCoroutine(MonsterTurn());
        }
    }

    IEnumerator MonsterTurn()
    {
        roundInProgress = false;

        yield return new WaitForSeconds(delayBeforeMonstersMove);
        monsterManager.MoveMonsters();

        yield return new WaitForSeconds(1f);

        if (monsterManager.AreAllMonstersDefeated())
        {
            AdvanceLevel();
        }
        else
        {
            StartCoroutine(ResetRound());
        }
    }

    void StartRound()
    {
        roundInProgress = true;
        throwsUsed = 0;
        dartThrowController.ResetThrows();
        uiManager?.UpdateThrowCount(throwsPerRound);
    }

    IEnumerator ResetRound()
    {
        yield return new WaitForSeconds(delayBeforeNextRound);
        StartRound();
    }

    void AdvanceLevel()
    {
        currentLevel++;
        monsterManager.SpawnMonstersForLevel(currentLevel);
        uiManager?.ShowLevelUp(currentLevel);

        StartCoroutine(ResetRound());
    }

    public int GetCurrentLevel() => currentLevel;

    public bool IsRoundInProgress() => roundInProgress; 
}
