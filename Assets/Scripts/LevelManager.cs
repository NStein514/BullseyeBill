using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject[] monsterPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public int monstersPerWave = 5;
    public float waveDelay = 5f;

    private bool isSpawning = true;
    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    System.Collections.IEnumerator SpawnWaves()
    {
        while (isSpawning)
        {
            currentWave++;
            yield return StartCoroutine(SpawnMonsters(monstersPerWave + currentWave));

            yield return new WaitForSeconds(waveDelay);

            // Potential difficulty scaling? 
            spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.1f);
        }
    }

    System.Collections.IEnumerator SpawnMonsters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameManager.Instance.isGameOver)
            {
                isSpawning = false;
                yield break;
            }

            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMonster()
    {
        if (monsterPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject monster = Instantiate(
            monsterPrefabs[Random.Range(0, monsterPrefabs.Length)],
            spawnPoints[Random.Range(0, spawnPoints.Length)].position,
            Quaternion.identity
        );
    }
}