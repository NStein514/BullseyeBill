using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    public List<MonsterController> monsters;

    public delegate void MonstersDefeatedEvent();
    public event MonstersDefeatedEvent OnMonstersDefeated;

    public void MoveMonsters()
    {
        // Simple example: just call a move or attack animation
        foreach (var monster in monsters)
        {
            // Placeholder: maybe trigger a move or attack animation
            Animator anim = monster.GetComponent<Animator>();
            if(anim != null)
                anim.SetTrigger("Move");
        }
    }

    public void CheckMonstersDefeated()
    {
        monsters.RemoveAll(m => m == null); // clean nulls after death

        if (monsters.Count == 0)
        {
            OnMonstersDefeated?.Invoke();
        }
    }
}
