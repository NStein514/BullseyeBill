using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    private Animator animator;

    public DartboardSections section;
    public Transform targetPosition;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (targetPosition != null)
        {
            transform.position = targetPosition.position;
            transform.rotation = targetPosition.rotation;
        }
    }

    public void Setup(DartboardSections section, Transform target)
    {
        this.section = section;
        this.targetPosition = target;

        if (targetPosition != null)
        {
            transform.position = targetPosition.position;
            transform.rotation = targetPosition.rotation;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");

        Destroy(gameObject, 0.5f); 
    }
}
