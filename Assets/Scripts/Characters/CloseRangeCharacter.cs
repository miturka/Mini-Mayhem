using UnityEngine;

public class CloseRangeCharacter : Character
{
    protected override void InitializeCharacter()
    {
        // Configure movement
        if (movement != null)
        {
            movement.moveSpeed = 5.1f;
            movement.jumpHeight = 2f;
        }

        // Configure health
        if (health != null)
        {
            health.maxHealth = 120;
            health.InitializeHealth();
        }

        primaryAbility = GetComponent<SpinAttack>(); 
        secondaryAbility = GetComponent<Shockwave>();   

        Debug.Log("CloseRangeCharacter initialized with SpinAttack ability.");
    }
}
