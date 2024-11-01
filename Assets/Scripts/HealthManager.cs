using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle player defeat (e.g., disable player, display KO message)
        Debug.Log(gameObject.name + " has been knocked out!");
        // Optionally, disable movement or trigger a knockout animation
        gameObject.SetActive(false); // Disables the player
    }
}
