using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    [Header("Monster Prefabs (1 through 6)")]
    public List<GameObject> monsterPrefabs;

    [Header("Spawn Points by Zone")]
    public List<Transform> bullseyePoints;
    public List<Transform> ring1Points;
    public List<Transform> triangle2Points;
    public List<Transform> ring3Points;
    public List<Transform> triangle4Points;
    public List<Transform> ring5Points;

    [Header("Dartboard Settings")]
    public Transform dartboardCenter; // Assign in Inspector!

    private MonsterController currentMonster;
    private List<MonsterController> activeMonsters = new List<MonsterController>();

    public delegate void AllMonstersDefeatedEvent();
    public event AllMonstersDefeatedEvent OnAllMonstersDefeated;

    public void SpawnMonsters(int level)
    {
        ClearMonster();

        int type = 1; // Default to bullseye for round 1
        if (level > 1)
        {
            type = Random.Range(2, 7); // Randomly pick 2 through 6 (inclusive)
        }

        GameObject prefab = monsterPrefabs[type - 1];
        Transform spawnPoint = GetSpawnPointForType(type);

        if (spawnPoint == null)
        {
            Debug.LogWarning("No spawn point found for monster type " + type);
            return;
        }

        GameObject monsterObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        currentMonster = monsterObj.GetComponent<MonsterController>();

        if (currentMonster != null)
        {
            bool rotateToCenter = (type >= 3 && type <= 6);
            currentMonster.Setup(spawnPoint, dartboardCenter, rotateToCenter);
            currentMonster.SetHealth(1);
            currentMonster.OnMonsterDefeated += HandleMonsterDefeated;

            activeMonsters.Add(currentMonster);
        }
    }

    Transform GetSpawnPointForType(int type)
    {
        switch (type)
        {
            case 1: return GetRandomPoint(bullseyePoints);
            case 2: return GetRandomPoint(ring1Points);
            case 3: return GetRandomPoint(triangle2Points);
            case 4: return GetRandomPoint(ring3Points);
            case 5: return GetRandomPoint(triangle4Points);
            case 6: return GetRandomPoint(ring5Points);
            default: return null;
        }
    }

    Transform GetRandomPoint(List<Transform> points)
    {
        if (points == null || points.Count == 0)
            return null;

        return points[Random.Range(0, points.Count)];
    }

    public void MoveOrAttackMonsters()
    {
        if (currentMonster != null)
        {
            Animator anim = currentMonster.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Move");
        }
    }

    public bool AreAllMonstersDefeated()
    {
        return activeMonsters.Count == 0;
    }

    private void HandleMonsterDefeated()
    {
        if (currentMonster != null)
        {
            currentMonster.OnMonsterDefeated -= HandleMonsterDefeated;
            activeMonsters.Remove(currentMonster);
            currentMonster = null;
        }

        if (activeMonsters.Count == 0)
        {
            OnAllMonstersDefeated?.Invoke();
        }
    }

    private void ClearMonster()
    {
        foreach (var monster in activeMonsters)
        {
            if (monster != null)
            {
                Destroy(monster.gameObject);
            }
        }

        activeMonsters.Clear();
        currentMonster = null;
    }

#if UNITY_EDITOR
    [ContextMenu("Test All Monster Spawns")]
    public void TestAllMonsterSpawns()
    {
        ClearMonster(); // Clear any previously spawned monsters

        for (int type = 1; type <= 6; type++)
        {
            List<Transform> spawnPoints = GetSpawnPointsListForType(type);
            GameObject prefab = monsterPrefabs[type - 1];

            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint == null || prefab == null) continue;

                GameObject monsterObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                monsterObj.name = $"MonsterType{type}_At_{spawnPoint.name}";

                MonsterController controller = monsterObj.GetComponent<MonsterController>();
                if (controller != null)
                {
                    bool rotateToCenter = (type >= 3 && type <= 6);
                    controller.Setup(spawnPoint, dartboardCenter, rotateToCenter);
                    controller.SetHealth(1);
                    activeMonsters.Add(controller);
                }
            }
        }
    }
#endif

    List<Transform> GetSpawnPointsListForType(int type)
    {
        switch (type)
        {
            case 1: return bullseyePoints;
            case 2: return ring1Points;
            case 3: return triangle2Points;
            case 4: return ring3Points;
            case 5: return triangle4Points;
            case 6: return ring5Points;
            default: return new List<Transform>();
        }
    }
}
