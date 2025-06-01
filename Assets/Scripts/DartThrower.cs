using System.Collections;
using UnityEngine;

public class DartThrower : MonoBehaviour
{
    public static DartThrower Instance;

    [Header("Dart Settings")]
    public GameObject dartPrefab;
    public Transform firePoint;
    public float throwForce = 25f;
    public float fireRate = 0.5f;

    private float fireCooldown;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            ThrowDart();
            fireCooldown = fireRate;
        }
    }

    public void ThrowDart()
    {
        if (dartPrefab == null || firePoint == null)
        {
            Debug.LogWarning("DartThrower is missing references.");
            return;
        }

        GameObject dart = Instantiate(dartPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = dart.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * throwForce;
        }
    }

    public IEnumerator EnableRapidFire(float duration)
    {
        float originalRate = fireRate;
        fireRate /= 2f; // or any other speed multiplier 
        yield return new WaitForSeconds(duration);
        fireRate = originalRate; 
    }
}
