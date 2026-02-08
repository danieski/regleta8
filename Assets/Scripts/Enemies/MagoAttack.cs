using UnityEngine;

public class MagoAttack : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float effectDelay = 0.5f, effectArea;
    public int damage = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Hit", effectDelay);
    }

    void Hit()
    {
        hitEffect.SetActive(true);
        Collider[] colliders = Physics.OverlapSphere(transform.position, effectArea);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<HealthComponent>().TakeDamage(damage);
            }
        }
        Invoke("Die", 0.5f);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, effectArea);
    }
}
