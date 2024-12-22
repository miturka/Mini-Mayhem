using UnityEngine;

public class ArenaBoundary : MonoBehaviour
{
    public float knockbackForce = 10f; // Force of the knockback

    public float speedMultiplier = 1.5f;

    public Vector3 knockbackDirectionOffset = new Vector3(0, 1, 0); 

    /*internal void ApplyKnockbackV2(CharacterController target)
    {
        Vector3 knockbackDirection = (Vector3.zero - target.transform.position).normalized + knockbackDirectionOffset;
        StartCoroutine(ApplyKnockbackCoroutineV2(target, knockbackDirection, knockbackForce));
    }

    private System.Collections.IEnumerator ApplyKnockbackCoroutineV2(CharacterController target, Vector3 direction, float force)
    {
        PlayerMovement targetMovement = target.gameObject.GetComponent<PlayerMovement>();
        targetMovement.FreezeMovement();

        // Zvýšenie rýchlosti pomocou multiplier
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z).normalized * force * speedMultiplier;
        float verticalVelocity = Mathf.Max(0, direction.y) * force * speedMultiplier;

        float knockbackTimer = 0.2f; // Čas, kedy je hráč "nútene vo vzduchu"
        bool forceAirborne = true;

        while (forceAirborne || !target.isGrounded)
        {
            if (knockbackTimer > 0)
            {
                knockbackTimer -= Time.deltaTime;
            }
            else
            {
                forceAirborne = false;
            }

            // Rýchlejšie pridávanie gravitácie (zrýchlený pohyb dole)
            verticalVelocity += Physics.gravity.y * Time.deltaTime * speedMultiplier;

            // Kombinácia zrýchlenej horizontálnej a vertikálnej zložky
            Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;

            // Rýchlejší pohyb
            target.Move(velocity * Time.deltaTime);

            yield return null;
        }

        targetMovement.UnfreezeMovement();
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Skontroluje, či objekt je hráč
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            playerMovement.ReceiveArenaKnockback( knockbackDirectionOffset, knockbackForce, speedMultiplier);
            Debug.Log("Leavol player;");
        }
        
    }
}
