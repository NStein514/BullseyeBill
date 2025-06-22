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

    public delegate void AllMonstersDefeatedEvent();
    public event AllMonstersDefeatedEvent OnAllMonstersDefeated;

    public void SpawnMonsters(int level)
    {
        ClearMonster();

        int type = Mathf.Clamp(level, 1, 6);
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
            bool rotateToCenter = (type >= 3 && type <= 6); // Triangle/outer ring monsters
            currentMonster.Setup(spawnPoint, dartboardCenter, rotateToCenter);
            currentMonster.SetHealth(1); // One-hit KO
            currentMonster.OnMonsterDefeated += HandleMonsterDefeated;
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

    private void HandleMonsterDefeated()
    {
        if (currentMonster != null)
            currentMonster.OnMonsterDefeated -= HandleMonsterDefeated;

        currentMonster = null;
        OnAllMonstersDefeated?.Invoke();
    }

    private void ClearMonster()
    {
        if (currentMonster != null)
        {
            Destroy(currentMonster.gameObject);
            currentMonster = null;
        }
    }
}
