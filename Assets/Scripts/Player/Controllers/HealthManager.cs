using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Image healthBar;
    
    public TextMeshProUGUI healthNum;

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has been defeated.");
        GameLogic gameLogic = FindObjectOfType<GameLogic>();
        if (gameLogic != null)
        {
            gameLogic.PlayerDied(gameObject);
        }
        gameObject.SetActive(false); // Disables the player
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }

        healthNum.text = Mathf.RoundToInt(currentHealth).ToString() ;
    }
}
