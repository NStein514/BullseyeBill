using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] monsterPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public bool spawnContinuously = true;

    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (!spawnContinuously || GameManager.Instance.isGameOver) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnMonster();
            spawnTimer = spawnInterval;
        }
    }

    public void SpawnMonster()
    {
        if (monsterPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }

    public void StartSpawning()
    {
        spawnContinuously = true;
    }

    public void StopSpawning()
    {
        spawnContinuously = false; 
    }
}