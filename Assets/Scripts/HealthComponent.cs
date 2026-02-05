using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 3;

    [SerializeField] private UnityEvent onDie;
    [SerializeField] private GameObject[] uiHearts;

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
        print("take damage");
        if (gameObject.GetComponent<CharacterStateManager>() != null)
        {
            CharacterStateManager character = gameObject.GetComponent<CharacterStateManager>();
            print("Soy invencible? " + character.isInvencible);
            if (character.isInvencible)
                return;
            character.OnDamage();

        }
        currentHealth -= damage;
        print("Current Health: " + currentHealth);
        for (int i = uiHearts.Length - 1; i >= 0; i--)
        {
            if (i < currentHealth)
            {
                uiHearts[i].SetActive(true);
            }
            else
            {
                uiHearts[i].SetActive(false);
            }
        }
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
