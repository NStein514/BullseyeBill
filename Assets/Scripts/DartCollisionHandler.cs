using UnityEngine;

public class DartCollisionHandler : MonoBehaviour
{
    public int baseDamage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        MonsterController monster = collision.gameObject.GetComponent<MonsterController>();
        if (monster != null)
        {
            monster.TakeDamage(baseDamage);

            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetParent(monster.transform);
        }

        GetComponent<Collider>().enabled = false; 
    }
}
