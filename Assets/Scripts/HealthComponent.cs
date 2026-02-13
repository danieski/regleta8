using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 3;
    public bool isInvencible = false;

    [SerializeField] private GameObject[] uiHearts;
    [SerializeField] private Animator animator;

    public void OnDie()
    {

        if(gameObject.GetComponent<PlayerScript>() != null)
        {
            gameObject.GetComponent<PlayerScript>().OnPlayerDie();
        }
        
        Destroy(gameObject);

    }
    public void TakeDamage(int damage)
    {
        // Check if is in the invulneravility state that shares with all enities
        if (isInvencible)
            return;
        // Set the invulnerability state to true for 1 second
        Invoke("OnInvencibilityDown", 1f);
        isInvencible = true; 
        currentHealth -= damage;
        //animation
        if (animator != null)
            animator.SetTrigger("hurt");

        if (uiHearts != null)
        {
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

    }

}
