using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Image healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);
        UpdateHealthBar();

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

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
