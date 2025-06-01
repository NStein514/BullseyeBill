using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("Monster Settings")]
    public float moveSpeed = 2f;
    public int maxHealth = 3;
    private int currentHealth;
    private bool isFrozen = false;

    [Header("Target")]
    public Transform target; // The Bullseye 

    void Start()
    {
        currentHealth = maxHealth;

        if (target == null)
        {
            GameObject bullseye = GameObject.FindGameObjectWithTag("Bullseye");
            if (bullseye != null)
                target = bullseye.transform;
            else
                Debug.LogWarning("Bullseye target not set and not found");
        }
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        if (isFrozen || isActiveAndEnabled) return; 
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.Instance.AddScore(10);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullseye"))
        {
            GameManager.Instance.LoseLife();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Dart"))
        {
            TakeDamage(1); // Adjust damage dealth as needed 
            Destroy(other.gameObject);
        }
    }

    public void Freeze(bool freeze)
    {
        isFrozen = freeze; 
    }
}