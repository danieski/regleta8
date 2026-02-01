using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 3;

    [SerializeField] private UnityEvent onDie;

    public void OnDie()
    {
        // Handle death logic here
        //Debug.Log("Entity has died.");
        if(gameObject.GetComponent<CharacterStateManager>() != null)
        {
            CharacterStateManager character = gameObject.GetComponent<CharacterStateManager>();
            character.SwitchState(character.deathState);
            return;
        }
        onDie.Invoke();
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
