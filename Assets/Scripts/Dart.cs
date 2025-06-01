using UnityEngine;

public class Dart : MonoBehaviour
{
    [Header("Dart Settings")]
    public int damage = 1;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Auto-destroy after a few seconds 
    }

    void OnTriggerEnter(Collider other)
    {
        // Only interacts with monsters
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy dart on hit
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("Dart"))
        {
            // Destroy the dart if it hits anything else 
            Destroy(gameObject); 
        }
    }
}
