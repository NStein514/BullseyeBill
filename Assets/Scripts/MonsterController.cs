using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public int health = 10;
    public float speed = 1f;

    public void Setup(int level)
    {
        health = 10 + level * 5;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public bool IsAlive() => health > 0;

    public void MoveTowardPlayer(float step)
    {
        transform.position += Vector3.forward * step; // Move toward +Z; change as needed
    }

    void Die()
    {
        // Play animation/sound if needed
        Destroy(gameObject, 0.1f);
    }
}
