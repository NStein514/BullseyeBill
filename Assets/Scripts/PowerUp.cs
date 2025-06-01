using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        ExtraLife,
        FreezeMonsters,
        RapidFire,
        ScoreBoost
    }

    [Header("PowerUp Settings")]
    public PowerUpType type;
    public float duration = 5f;
    public int amount = 1;

    [Header("Effects")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivatePowerUp();
            PlayEffect();
            Destroy(gameObject);
        }
    }

    void ActivatePowerUp()
    {
        switch (type)
        {
            case PowerUpType.ExtraLife:
                GameManager.Instance.GainLife(amount);
                break;

            case PowerUpType.FreezeMonsters:
                StartCoroutine(GameManager.Instance.FreezeMonsters(duration));
                break;

            case PowerUpType.RapidFire:
                StartCoroutine(DartThrower.Instance.EnableRapidFire(duration));
                break;

            case PowerUpType.ScoreBoost:
                GameManager.Instance.AddScore(amount * 100);
                break;
        }
    }

    void PlayEffect()
    {
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position); 
    }
}
