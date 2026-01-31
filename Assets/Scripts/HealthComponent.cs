using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 3;

    public void OnDie()
    {
        // Handle death logic here
        Debug.Log("Entity has died.");
        Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnDie();
        }
    }
    public void OnHeal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

}
