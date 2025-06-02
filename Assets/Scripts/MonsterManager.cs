using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    [Header("Monster Setup")]
    public GameObject monsterPrefab;
    public Transform spawnAreaCenter;
    public float spawnRadius = 3f;
    public int baseMonsterCount = 3;
    public float moveStep = 1f;

    private List<MonsterController> activeMonsters = new List<MonsterController>();

    public void SpawnMonstersForLevel(int level)
    {
        ClearMonsters();

        int monsterCount = baseMonsterCount + (level - 1);
        for (int i = 0; i < monsterCount; i++)
        {
            Vector3 spawnPos = GetSpawnPosition(i, monsterCount);
            GameObject monsterGO = Instantiate(monsterPrefab, spawnPos, Quaternion.identity, transform);

            MonsterController mc = monsterGO.GetComponent<MonsterController>();
            mc.Setup(level); // Let monster scale with level
            activeMonsters.Add(mc);
        }
    }

    private Vector3 GetSpawnPosition(int index, int total)
    {
        float angle = (360f / total) * index;
        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        return spawnAreaCenter.position + dir * spawnRadius;
    }

    public void MoveMonsters()
    {
        foreach (var monster in activeMonsters)
        {
            if (monster != null && monster.IsAlive())
                monster.MoveTowardPlayer(moveStep);
        }
    }

    public bool AreAllMonstersDefeated()
    {
        return activeMonsters.TrueForAll(m => m == null || !m.IsAlive());
    }

    public void ClearMonsters()
    {
        foreach (var m in activeMonsters)
        {
            if (m != null)
                Destroy(m.gameObject);
        }
        activeMonsters.Clear();
    }
}
