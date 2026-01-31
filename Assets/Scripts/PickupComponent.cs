using UnityEngine;

public class PickupComponent : MonoBehaviour
{
    
    public PickupExample pickupVariable; 
    private void OnTriggerEnter(Collider other)
    {
    print("PickupComponent collided with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
       {
           other.gameObject.GetComponent<HealthComponent>().OnHeal(pickupVariable.healAmount);
           Destroy(gameObject);
        }
    }
}
