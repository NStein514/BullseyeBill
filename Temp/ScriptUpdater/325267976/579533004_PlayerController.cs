using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Dart Settings")]
    public GameObject dartPrefab;
    public Transform firePoint;
    public float throwForce = 25f;
    public float fireRate = 0.5f;

    private float fireCooldown = 0f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (firePoint == null)
            Debug.LogWarning("Fire Point not Assigned to PlayerController");
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            ThrowDart();
            fireCooldown = fireRate;
        }

        AimAtMouse();
    }

    void ThrowDart()
    {
        if (dartPrefab == null || firePoint == null) return;

        GameObject dart = Instantiate(dartPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = dart.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * throwForce;
        }
    }

    void AimAtMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetDirection = (hit.point - firePoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            firePoint.rotation = Quaternion.Lerp(firePoint.rotation, lookRotation, Time.deltaTime * 20f); 
        }
    }
}
