using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Collider hitbox;                  // Reference to the hitbox collider
    public int attackDamage = 10;            // Damage dealt per attack
    public float attackDuration = 0.2f;
    public float spinDuration = 0.5f; // How long the hitbox stays active
    public KeyCode attackKey = KeyCode.U;    // Configurable attack key (default is "U")

    private bool isAttacking = false;

    void Start()
    {
        // Ensure hitbox is disabled by default
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }

    void Update()
    {
        // Check if the configured attack key is pressed and if the player is not already attacking
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            Debug.Log("Starting attack");
            StartCoroutine(PerformAttack());
        }
    }


    private System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        hitbox.enabled = true;  // Enable the hitbox for the attack

        yield return StartCoroutine(SpinAnimation());
        yield return new WaitForSeconds(attackDuration);

        hitbox.enabled = false; // Disable the hitbox after the attack
        isAttacking = false;
    }

    private System.Collections.IEnumerator SpinAnimation()
    {
        float elapsedTime = 0f;
        float initialRotation = transform.eulerAngles.y;
        float targetRotation = initialRotation + 360f;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / spinDuration);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRotation, transform.eulerAngles.z);
            yield return null;
        }

        // Ensure the rotation is exactly 360 degrees after the spin
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetRotation, transform.eulerAngles.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // This should only trigger if the hitbox is enabled during an attack
        Debug.Log("Hit detected with: " + other.name); // Check what object it hit

        if (other.CompareTag("Player") && other.gameObject != gameObject)
        {
            Debug.Log("Hit another player");
            HealthManager opponentHealth = other.GetComponent<HealthManager>();
            if (opponentHealth != null)
            {
                opponentHealth.TakeDamage(attackDamage);
            }
        }
    }
}
