using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementt : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody rb;
    private Vector3 movement;

    public float jumpForce = 5f; 
    public float gravityMultiplier = 2f;

    [SerializeField]
    private bool isGrounded = true;

    public float dodgeSpeed = 1f; 
    public float dodgeDuration = 0.1f;
    public float dodgeCooldown = 1f; 
    private bool isDodging = false;
    private float lastDodgeTime = -1f;

    private Vector3 isometricForward = new Vector3(1, 0, 1).normalized; 
    private Vector3 isometricRight = new Vector3(1, 0, -1).normalized;

    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    void Update()
    {
        Vector3 forwardMovement = Input.GetAxisRaw("Vertical") * isometricForward;
        Vector3 rightMovement = Input.GetAxisRaw("Horizontal") * isometricRight;
        movement = (forwardMovement + rightMovement).normalized;

        isGrounded = IsGrounded();

        // Smooth rotation
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime); 
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Dodge
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDodgeTime + dodgeCooldown && !isDodging)
        {
            StartCoroutine(Dodge());
        }
    }

    void FixedUpdate()
    {
        // Normal movement only if not dodging
        if (!isDodging) 
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        // Apply extra gravity to bring the player down faster while jumping
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; 
    }

    IEnumerator Dodge()
    {
        isDodging = true; 
        lastDodgeTime = Time.time; 

        Vector3 dodgeDirection = transform.forward;

        float dodgeEndTime = Time.time + dodgeDuration;
        while (Time.time < dodgeEndTime)
        {
            rb.MovePosition(rb.position + dodgeDirection * dodgeSpeed * Time.fixedDeltaTime);
            yield return null; // Wait for the next frame
        }

        isDodging = false;
    }

    bool IsGrounded()
    {
        // Start the raycast from just below the player's transform position
        Vector3 rayOrigin = transform.position + Vector3.down * 0.1f;

        // Draw a ray in the Scene view to visually confirm its direction and distance
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);

        // Perform the Raycast and check if it hits the ground
        bool grounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayer);
        Debug.Log("IsGrounded Raycast Hit: " + grounded);
        return grounded;
    }


}
