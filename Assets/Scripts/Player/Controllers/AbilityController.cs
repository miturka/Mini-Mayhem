using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    private Player character;

    public KeyCode primaryAbilityKey = KeyCode.Q;
    public KeyCode secondaryAbilityKey = KeyCode.E;


    [Header("Cooldown UI")]
    public Image primaryCooldownBar;   // Assign in Inspector
    public Image secondaryCooldownBar; // Assign in Inspector

    void Start()
    {
        character = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(primaryAbilityKey))
        {
            character.UsePrimaryAbility();
        }

        if (Input.GetKeyDown(secondaryAbilityKey))
        {
            character.UseSecondaryAbility();
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
    }

    private void UpdateAbilityCooldown(BaseAbility ability, Image cooldownBar)
    {
        
        if (ability.IsOnCooldown())
        {
            // Calculate remaining cooldown percentage
            float elapsedTime = Time.time - ability.lastActivationTime;
            float cooldownPercentage = Mathf.Clamp01(elapsedTime / ability.GetCooldown());

            // Update the UI bar (inverted because we are filling it)
            cooldownBar.fillAmount = 0 + cooldownPercentage;
        }
        else
        {
            // Ability is ready, so set the bar to full
            cooldownBar.fillAmount = 1;
        }
    }
}
