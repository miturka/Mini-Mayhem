using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody rb;
    private Vector3 movement;

    public float jumpForce = 5f; 
    public float gravityMultiplier = 2f; 
    private bool isGrounded = true;

    public float dodgeSpeed = 1f; 
    public float dodgeDuration = 0.1f;
    public float dodgeCooldown = 1f; 
    private bool isDodging = false;
    private float lastDodgeTime = -1f;

    private Vector3 isometricForward = new Vector3(1, 0, 1).normalized; 
    private Vector3 isometricRight = new Vector3(1, 0, -1).normalized;

    void Update()
    {
        Vector3 forwardMovement = Input.GetAxisRaw("Vertical") * isometricForward;
        Vector3 rightMovement = Input.GetAxisRaw("Horizontal") * isometricRight;
        movement = (forwardMovement + rightMovement).normalized;

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; 
        }
    }
}
