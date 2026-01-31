using UnityEngine;

public class HurtComponent : MonoBehaviour
{

    public int damage = 1;
    void Start()
    {
        
    }

    void Update()
    {
    }
    private void OnTriggerEnter(Collider collision)
    {

        print("Me: "+gameObject.name+" HurtComponent collided with " + collision.gameObject.name);
        HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damage);
        }
    }

}
