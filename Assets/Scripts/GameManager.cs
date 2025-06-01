using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerLives = 3;
    public int score = 0;
    public bool isGameOver = false;

    public Text scoreText;
    public Text livesText;
    public GameObject gameOverUI;

    void Awake()
    {
        // Ensure only one GameManager exists
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

    void Start()
    {
        gameOverUI.SetActive(false);
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        UIManager.Instance.UpdateScore(score);
    }

    public void LoseLife(int damageToPlayer)
    {
        if (isGameOver) return;

        playerLives--;
        UIManager.Instance.UpdateLives(playerLives);

        if (playerLives <= 0)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        isGameOver = true;
        UIManager.Instance.ShowGameOver();
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    internal void LoseLife()
    {
        throw new NotImplementedException();
    }

    public void GainLife(int amount)
    {
        playerLives += amount;
        UIManager.Instance.UpdateLives(playerLives);
    }

    public IEnumerator FreezeMonsters(float duration)
    {
        MonsterController[] monsters = FindObjectsOfType<MonsterController>();

        foreach (var m in monsters)
            m.Freeze(true);

        yield return new WaitForSeconds(duration);

        foreach (var m in monsters)
            m.Freeze(false); 
    }
}
