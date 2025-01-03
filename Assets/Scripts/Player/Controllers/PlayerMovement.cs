using System.Collections;
using UnityEngine;

//Manages movement of a player
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

    public bool isBasicKBRunning;   // is knockback from ability being applied

    public bool isFrozen = false;
    public bool isRapidFiring = false;
    public bool isArenaKnockbackActive = false; // is knockback from arena being applied

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

        // Get movement directions
        if (Input.GetKey(upKey)) movement += isometricForward;
        if (Input.GetKey(downKey)) movement -= isometricForward;
        if (Input.GetKey(leftKey)) movement -= isometricRight;
        if (Input.GetKey(rightKey)) movement += isometricRight;

        // Apply speed multiplier
        movement = movement.normalized * (currentSpeed * speedMultiplier);

        // toggle animations
        if (movement != Vector3.zero)
        {
            animator.SetBool("isWalking", true); // Spusti chôdzu
        }
        else
        {
            animator.SetBool("isWalking", false); // Prejdi do idle
        }
        

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
            return; // dont allow other knockback when arena is giving knockback
        }
        Vector3 knockbackDirection = (transform.position - knockbackOrigin).normalized + knockbackDirectionOffset;
        if (currentKnockbackCoroutine != null)
        {
            StopCoroutine(currentKnockbackCoroutine); // stop current knockback
        }
        // apply new knockback
        currentKnockbackCoroutine = StartCoroutine(ApplyKnockbackCoroutine(knockbackDirection, knockbackForce, speedMultiplier));
    }

    internal void ReceiveArenaKnockback(Vector3 knockbackDirectionOffset, float knockbackForce, float speedMultiplier)
    {
        // apply knockback when trying to leave the arena
        Vector3 knockbackDirection = (Vector3.zero - transform.position).normalized + knockbackDirectionOffset;
        isArenaKnockbackActive = true;
        if (currentKnockbackCoroutine != null)
        {
            StopCoroutine(currentKnockbackCoroutine); // stop current knockback
            Debug.Log("zastavujem");

        }
        else{
            Debug.Log("iny knockback nebezi");
        }
        // apply arena knockback
        currentKnockbackCoroutine = StartCoroutine(ApplyArenaKnockbackCoroutine( knockbackDirection, knockbackForce, speedMultiplier));
    }

    private System.Collections.IEnumerator ApplyKnockbackCoroutine(Vector3 direction, float force, float speedMultiplier)
    {
        
        FreezeMovement();
        isBasicKBRunning = true; 

        // calculate velocities
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z).normalized * force * speedMultiplier;
        float verticalVelocity = Mathf.Max(0, direction.y) * force * speedMultiplier;

        float knockbackTimer = 0.2f; 
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

            // recalculate velocity
            verticalVelocity += Physics.gravity.y * Time.deltaTime * speedMultiplier;
            Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;

            //apply knockback movement
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

        // calculate velocities
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z).normalized * force * speedMultiplier;
        float verticalVelocity = Mathf.Max(0, direction.y) * force * speedMultiplier;

        float knockbackTimer = 0.2f; 
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

            // recalculate velocity
            verticalVelocity += Physics.gravity.y * Time.deltaTime * speedMultiplier;
            Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;

            // apply arena knockback movement
            controller.Move(velocity * Time.deltaTime);

            yield return null;
        }

        isArenaKnockbackActive = false; 
        UnfreezeMovement();
        currentKnockbackCoroutine = null;
    }
}
