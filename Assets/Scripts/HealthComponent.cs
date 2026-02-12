using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 3;
    public bool isInvencible = false;

    [SerializeField] private UnityEvent onDie;
    [SerializeField] private GameObject[] uiHearts;
    [SerializeField] private Animator animator;

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
        print("Soy invencible? " + name + isInvencible);
        // Check if is in the invulneravility state that shares with all enities
        if (isInvencible)
            return;
        // Set the invulnerability state to true for 1 second
        isInvencible = true; 
        Invoke("OnInvencibilityDown", 5f);
        currentHealth -= damage;
        //animation
        if (animator != null)
            animator.SetTrigger("hurt");
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
    public void OnInvencibilityDown()
    {
        isInvencible = false;
        print("Ya no soy invencible " + name);
    }

}
