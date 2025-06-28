using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    [Header("Face Parts")]
    public SpriteRenderer leftEyeRenderer;
    public SpriteRenderer rightEyeRenderer;
    public SpriteRenderer mouthRenderer;

    public Sprite[] leftEyeSprites;
    public Sprite[] rightEyeSprites;
    public Sprite[] mouthSprites;

    [Header("Target Info")]
    public Transform targetPosition;
    public bool rotateTowardCenter = false;
    public Transform dartboardCenter;

    public delegate void MonsterDefeatedEvent();
    public event MonsterDefeatedEvent OnMonsterDefeated;

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
            OrientTowardDartboardCenter();
        else if (target != null)
            transform.rotation = target.rotation;

        RandomizeFaceParts();
    }

    void OrientTowardDartboardCenter()
    {
        Vector3 toCenter = dartboardCenter.position - transform.position;
        float angle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void RandomizeFaceParts()
    {
        if (leftEyeSprites.Length >= 6 && rightEyeSprites.Length >= 6 && mouthSprites.Length >= 6)
        {
            leftEyeRenderer.sprite = leftEyeSprites[Random.Range(0, 6)];
            rightEyeRenderer.sprite = rightEyeSprites[Random.Range(0, 6)];
            mouthRenderer.sprite = mouthSprites[Random.Range(0, 6)];
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Set all face parts to X versions (index 6)
        if (leftEyeSprites.Length > 6) leftEyeRenderer.sprite = leftEyeSprites[6];
        if (rightEyeSprites.Length > 6) rightEyeRenderer.sprite = rightEyeSprites[6];
        if (mouthSprites.Length > 6) mouthRenderer.sprite = mouthSprites[6];

        OnMonsterDefeated?.Invoke();
        Destroy(gameObject, 0.5f);
    }
}
