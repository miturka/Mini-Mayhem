using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float currentSpeed;
    private float speedMultiplier = 1f; // Multiplier for speed changes
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 2f;
    private Vector3 movement;
    private Vector3 velocity;

    private CharacterController controller;

    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.1f;
    public float dodgeCooldown = 1f;
    private bool isDodging = false;
    private float lastDodgeTime = -1f;

    private Vector3 isometricForward = new Vector3(1, 0, 1).normalized;
    private Vector3 isometricRight = new Vector3(1, 0, -1).normalized;

    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dodgeKey = KeyCode.LeftShift;

    public Animator animator;

    private Coroutine currentKnockbackCoroutine;

    public bool isBasicKBRunning;

    public bool isFrozen = false;
    public bool isRapidFiring = false;

    public bool isArenaKnockbackActive = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed; // Initialize currentSpeed with the base moveSpeed
    }

    void Update()
    {

        movement = Vector3.zero;

        if (isFrozen)
        {
            if (isRapidFiring)
            {
                HandleRotation(200.0f);
            }
            return;
        }

        if (Input.GetKey(upKey)) movement += isometricForward;
        if (Input.GetKey(downKey)) movement -= isometricForward;
        if (Input.GetKey(leftKey)) movement -= isometricRight;
        if (Input.GetKey(rightKey)) movement += isometricRight;

        // Apply speed multiplier
        movement = movement.normalized * (currentSpeed * speedMultiplier);

        if (movement != Vector3.zero)
        {
            animator.SetBool("isWalking", true); // Spusti chôdzu
        }
        else
        {
            animator.SetBool("isWalking", false); // Prejdi do idle
        }
        

        // Use CharacterController's built-in isGrounded property
        if (controller.isGrounded)
        {
            velocity.y = -2f; // Small downward force to keep player grounded

            // Jump
            if (Input.GetKey(jumpKey))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // Apply gravity when not grounded
            velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        }

        // Smooth rotation
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
        }

        // Dodge
        if (Input.GetKeyDown(dodgeKey) && Time.time >= lastDodgeTime + dodgeCooldown && !isDodging)
        {
            StartCoroutine(Dodge(movement));
        }

        // Apply movement and gravity through CharacterController
        if (!isDodging)
        {
            controller.Move((movement + velocity) * Time.deltaTime);
        }
    }

    IEnumerator Dodge(Vector3 dodgeDirection)
    {
        isDodging = true;
        lastDodgeTime = Time.time;

        float dodgeEndTime = Time.time + dodgeDuration;
        while (Time.time < dodgeEndTime)
        {
            controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            yield return null;
        }

        isDodging = false;
    }

    public void FreezeMovement()
    {
        isFrozen = true;
        animator.SetBool("isWalking", false); 
    }

    public void UnfreezeMovement()
    {
        isFrozen = false;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1f;
    }

    public void RapidFireEnabled()
    {
        moveSpeed /= 2;
    }

    public void RapidFireDisabled()
    {
        moveSpeed *= 2;
    }

    public void HandleRotation(float rotationSpeed)
    {
        if (Input.GetKey(leftKey))
        {
            controller.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(rightKey))
        {
            controller.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    internal void ReceiveKnockback( Vector3 knockbackOrigin, Vector3 knockbackDirectionOffset, float knockbackForce, float speedMultiplier)
    {
        if (isArenaKnockbackActive)
        {
            Debug.Log("Arena knockback already active.");
            return; // Nepovoľ ďalší arenový knockback
        }
        Vector3 knockbackDirection = (transform.position - knockbackOrigin).normalized + knockbackDirectionOffset;
        if (currentKnockbackCoroutine != null)
        {
            StopCoroutine(currentKnockbackCoroutine); // Zastav aktuálny knockback
        }
        currentKnockbackCoroutine = StartCoroutine(ApplyKnockbackCoroutine(knockbackDirection, knockbackForce, speedMultiplier));
    }

    internal void ReceiveArenaKnockback(Vector3 knockbackDirectionOffset, float knockbackForce, float speedMultiplier)
    {
        Debug.Log("ARENOVYY");
        Vector3 knockbackDirection = (Vector3.zero - transform.position).normalized + knockbackDirectionOffset;
        isArenaKnockbackActive = true;
        if (currentKnockbackCoroutine != null)
        {
            StopCoroutine(currentKnockbackCoroutine); // Zastav aktuálny knockback
            Debug.Log("zastavujem knokbak z idk ani");

        }
        else{
            Debug.Log("iny knok nebezi");
        }
        currentKnockbackCoroutine = StartCoroutine(ApplyArenaKnockbackCoroutine( knockbackDirection, knockbackForce, speedMultiplier));
    }

    private System.Collections.IEnumerator ApplyKnockbackCoroutine(Vector3 direction, float force, float speedMultiplier)
    {
        
        FreezeMovement();
        isBasicKBRunning = true; 
        // Zvýšenie rýchlosti pomocou multiplier
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z).normalized * force * speedMultiplier;
        float verticalVelocity = Mathf.Max(0, direction.y) * force * speedMultiplier;

        float knockbackTimer = 0.2f; // Čas, kedy je hráč "nútene vo vzduchu"
        bool forceAirborne = true;

        while (forceAirborne || !controller.isGrounded)
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
            controller.Move(velocity * Time.deltaTime);

            yield return null;
        }

        UnfreezeMovement();
        currentKnockbackCoroutine = null;
        isBasicKBRunning = false; 
    }

    private System.Collections.IEnumerator ApplyArenaKnockbackCoroutine(Vector3 direction, float force, float speedMultiplier)
    {
        FreezeMovement();

        // Zvýšenie rýchlosti pomocou multiplier
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z).normalized * force * speedMultiplier;
        float verticalVelocity = Mathf.Max(0, direction.y) * force * speedMultiplier;

        float knockbackTimer = 0.2f; // Čas, kedy je hráč "nútene vo vzduchu"
        bool forceAirborne = true;

        while (forceAirborne || !controller.isGrounded)
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
            controller.Move(velocity * Time.deltaTime);

            yield return null;
        }

        isArenaKnockbackActive = false; 
        UnfreezeMovement();
        currentKnockbackCoroutine = null;
    }
}
