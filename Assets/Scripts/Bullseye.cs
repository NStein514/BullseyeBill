using UnityEngine;

public class Bullseye : MonoBehaviour
{
    [Header("Bullseye Settings")]
    public int damageToPlayer = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            GameManager.Instance.LoseLife(); 
            Destroy(other.gameObject); 
        }
    }
}
