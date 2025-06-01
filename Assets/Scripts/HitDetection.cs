using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [Header("Hit Settings")]
    public int damage = 1;
    public GameObject hitEffect;
    public AudioClip hitSound;
    public float destroyDelay = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            TriggerEffects();
            Destroy(gameObject, destroyDelay);
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("Dart"))
        {
            TriggerEffects();
            Destroy(gameObject, destroyDelay);
        }
    }

    void TriggerEffects()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position); 
        }
    }
}
