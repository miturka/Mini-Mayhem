using UnityEngine;

public class AbilityController : MonoBehaviour
{
    private Character character;

    public KeyCode primaryAbilityKey = KeyCode.Q;
    public KeyCode secondaryAbilityKey = KeyCode.E;

    void Start()
    {
        character = GetComponent<Character>();
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
    }
}
