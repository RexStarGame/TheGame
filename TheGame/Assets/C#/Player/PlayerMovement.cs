using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement
    public float speed;
    public float jump;
    private float moveVelocity;

    // Downward force multiplier for falling
    public float fallForceMultiplier = 10f;

    // Grounded check
    private bool grounded = true;
    [SerializeField] private Transform groundCheck; // Tjekpunkt under spilleren
    [SerializeField] private float groundCheckRadius = 0.2f;

    [SerializeField] private List<string> allowedLayerNames; // Liste med lag-navne

    private Rigidbody2D rb;
    private int faceDirection = 1;

    void Start()
    {
        // Initialize Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the player!");
        }
    }

    private bool IsGrounded()
    {
        // Brug en OverlapCircle til at tjekke, om spilleren rører jorden
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);

        foreach (var hit in hits)
        {
            if (allowedLayerNames.Contains(LayerMask.LayerToName(hit.gameObject.layer)))
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (grounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jump);
            }
        }

        moveVelocity = 0;

        // Left movement
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveVelocity = -speed;

            // Flip if moving left
            if (faceDirection != -1)
            {
                flip();
                faceDirection = -1;
            }
        }

        // Right movement
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveVelocity = speed;

            // Flip if moving right
            if (faceDirection != 1)
            {
                flip();
                faceDirection = 1;
            }
        }

        // Apply horizontal movement
        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
    }

    void FixedUpdate()
    {
        // Opdater grounded-status
        grounded = IsGrounded();

        // Tilføj faldkraft, hvis spilleren ikke er på jorden
        if (rb.velocity.y < -1f && !grounded)
        {
            rb.AddForce(Vector2.down * fallForceMultiplier, ForceMode2D.Force);
        }
    }

    // Flip the player's sprite
    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Debugging - Tegn GroundCheck radius
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
