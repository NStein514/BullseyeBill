using UnityEngine;

public class DartCollisionHandler : MonoBehaviour
{
    private bool hasHit = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        // Stop dart movement
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Stick the dart at the hit point
        ContactPoint contact = collision.contacts[0];
        transform.position = contact.point;
        transform.rotation = Quaternion.LookRotation(-contact.normal);

        // Handle score zone detection
        DartboardZone zone = collision.collider.GetComponent<DartboardZone>();
        if (zone != null)
        {
            int baseValue = zone.baseScore; // e.g. 20, 15, etc.
            int multiplier = zone.multiplier; // 1 (normal), 2 (double), 3 (triple)
            int damage = baseValue * multiplier;

            // Optionally: play sound or visual effect here

            DealDamageToMonster(contact.point, damage);
        }
        else
        {
            Debug.Log("Hit something without a DartboardZone component.");
        }

        // Optionally: destroy dart after delay
        Destroy(gameObject, 2f);
    }

    void DealDamageToMonster(Vector3 hitPoint, int damage)
    {
        Ray ray = new Ray(hitPoint - transform.forward * 0.1f, transform.forward); // Slight back offset to avoid dartboard
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            MonsterController monster = hit.collider.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                Debug.Log($"Hit monster for {damage} damage.");
            }
        }
    }
}
