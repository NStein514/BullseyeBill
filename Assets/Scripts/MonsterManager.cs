using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    public GameObject monsterPrefab;

    [System.Serializable]
    public struct SectionSpawnPoint
    {
        public DartboardSections section;
        public Transform spawnPoint;
    }

    public List<SectionSpawnPoint> sectionSpawnPoints;

    private Dictionary<DartboardSections, Transform> spawnPointMap;
    public List<MonsterController> monsters = new List<MonsterController>();

    public delegate void MonstersDefeatedEvent();
    public event MonstersDefeatedEvent OnMonstersDefeated;

    void Awake()
    {
        spawnPointMap = new Dictionary<DartboardSections, Transform>();
        foreach (var entry in sectionSpawnPoints)
        {
            spawnPointMap[entry.section] = entry.spawnPoint;
        }
    }

    public void SpawnMonsters(List<DartboardSections> sectionsToSpawn)
    {
        ClearMonsters();

        foreach (DartboardSections section in sectionsToSpawn)
        {
            if (spawnPointMap.TryGetValue(section, out Transform spawnPoint))
            {
                GameObject monsterGO = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                MonsterController monster = monsterGO.GetComponent<MonsterController>();

                if (monster != null)
                {
                    monster.Setup(section, spawnPoint);
                    monsters.Add(monster);
                }
            }
            else
            {
                Debug.LogWarning($"No spawn point defined for section: {section}");
            }
        }
    }

    public void MoveMonsters()
    {
        foreach (var monster in monsters)
        {
            if (monster != null)
            {
                Animator anim = monster.GetComponent<Animator>();
                if (anim != null)
                    anim.SetTrigger("Move");
            }
        }
    }

    public void CheckMonstersDefeated()
    {
        monsters.RemoveAll(m => m == null);

        if (monsters.Count == 0)
        {
            OnMonstersDefeated?.Invoke();
        }
    }

    public void ClearMonsters()
    {
        foreach (var monster in monsters)
        {
            if (monster != null)
                Destroy(monster.gameObject);
        }

        monsters.Clear();
    }
}
