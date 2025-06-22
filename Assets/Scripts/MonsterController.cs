using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    [Header("Animation")]
    private Animator animator;

    [Header("Target Info")]
    public Transform targetPosition;
    public bool rotateTowardCenter = false;
    public Transform dartboardCenter; // Assign via MonsterManager

    public delegate void MonsterDefeatedEvent();
    public event MonsterDefeatedEvent OnMonsterDefeated;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetHealth(int health)
    {
        maxHealth = health;
        currentHealth = health;
        isDead = false;
    }

    public void Setup(Transform target, Transform boardCenter, bool rotateToCenter)
    {
        if (target != null)
        {
            targetPosition = target;
            transform.position = target.position;
        }

        dartboardCenter = boardCenter;
        rotateTowardCenter = rotateToCenter;

        if (rotateTowardCenter && dartboardCenter != null)
        {
            OrientTowardDartboardCenter();
        }
        else if (target != null)
        {
            transform.rotation = target.rotation;
        }
    }

    void OrientTowardDartboardCenter()
    {
        Vector3 toCenter = dartboardCenter.position - transform.position;

        // Only use x/y to rotate around z (keep sprite upright)
        float angle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        OnMonsterDefeated?.Invoke();
        Destroy(gameObject, 0.5f);
    }
}
