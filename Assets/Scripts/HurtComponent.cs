using UnityEngine;

public class HurtComponent : MonoBehaviour
{
    [SerializeField] private bool imPlayer = false;
    public int damage = 1;
    private void OnTriggerEnter(Collider collision)
    {
        if (imPlayer && collision.CompareTag("Player") || !imPlayer && !collision.CompareTag("Player")) return;
        //print("Me: "+gameObject.name+" HurtComponent collided with " + collision.gameObject.name);
        HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damage);
        }
    }

}
