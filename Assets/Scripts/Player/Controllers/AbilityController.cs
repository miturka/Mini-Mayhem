using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    private Player character;

    public KeyCode primaryAbilityKey = KeyCode.Q;
    public KeyCode secondaryAbilityKey = KeyCode.E;
    public KeyCode tertiaryAbilityKey = KeyCode.R;


    [Header("Cooldown UI")]
    public Image primaryCooldownBar;  
    public Image secondaryCooldownBar; 
    public Image tertiaryCooldownBar;

    void Start()
    {
        character = GetComponent<Player>();
    }

    void Update()
    {
        // control abilities of a player
        if (Input.GetKeyDown(primaryAbilityKey))
        {
            character.UsePrimaryAbility();
        }

        if (Input.GetKeyDown(secondaryAbilityKey))
        {
            character.UseSecondaryAbility();
        }

        if (Input.GetKeyDown(tertiaryAbilityKey))
        {
            character.UseTertiaryAbility();
        }

        // Update the cooldown UI
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        // Primary Ability Cooldown
        if (character.primaryAbility != null && primaryCooldownBar != null)
        {
            UpdateAbilityCooldown(character.primaryAbility, primaryCooldownBar);
        }

        // Secondary Ability Cooldown
        if (character.secondaryAbility != null && secondaryCooldownBar != null)
        {
            UpdateAbilityCooldown(character.secondaryAbility, secondaryCooldownBar);
        }

        if (character.tertiaryAbility != null && tertiaryCooldownBar != null)
        {
            UpdateAbilityCooldown(character.tertiaryAbility, tertiaryCooldownBar);
        }
    }

    private void UpdateAbilityCooldown(BaseAbility ability, Image cooldownBar)
    {
        
        if (ability.IsOnCooldown())
        {
            // Calculate remaining cooldown percentage
            float elapsedTime = Time.time - ability.lastActivationTime;
            float cooldownPercentage = Mathf.Clamp01(elapsedTime / ability.GetCooldown());

            // Update the UI bar 
            cooldownBar.fillAmount = 1 - cooldownPercentage;
        }
        else
        {
            // Ability is ready
            cooldownBar.fillAmount = 0;
        }
    }
}
