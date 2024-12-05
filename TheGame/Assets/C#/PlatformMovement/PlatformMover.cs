using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA; // Startpunkt
    public Transform pointB; // Slutpunkt
    public float speed = 2f; // Hastighed for bevægelsen

    [Header("Threshold Settings")]
    public float switchThreshold = 5f; // Tolerance for afstandstjek (kan justeres i Inspector)

    [Header("Friction Settings")]
    public PhysicsMaterial2D platformMaterial; // Physics Material til friktionsjustering
    public float customFriction = 0.4f; // Justér friktionen fra Inspector

    private Rigidbody2D rb; // Platformens Rigidbody2D
    private Transform currentTarget; // Det nuværende mål, platformen bevæger sig mod
    private Collider2D platformCollider; // Collider for platformen

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;

        platformCollider = GetComponent<Collider2D>();
        if (platformCollider == null)
        {
            platformCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        if (platformMaterial == null)
        {
            platformMaterial = new PhysicsMaterial2D("PlatformMaterial");
        }
        platformMaterial.friction = customFriction;
        platformCollider.sharedMaterial = platformMaterial;

        // Start med at bevæge mod pointA
        currentTarget = pointA;
        Debug.Log("Starting movement towards Point A");
    }

    void FixedUpdate()
    {
        // Bevæg platformen mod currentTarget
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        // Debugging: Log position og afstand
        //Debug.Log($"Moving towards {currentTarget.name} - Current Position: {transform.position} - Distance: {Vector2.Distance(transform.position, currentTarget.position)}");

        // Skift mål, når platformen når tæt nok på currentTarget
        if (Vector2.Distance(transform.position, currentTarget.position) < switchThreshold)
        {
            Debug.Log($"Reached {currentTarget.name}. Switching target...");
            currentTarget = currentTarget == pointA ? pointB : pointA;
            Debug.Log($"New target is {currentTarget.name}");
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }

    public void UpdateFriction(float newFriction)
    {
        customFriction = newFriction;
        if (platformMaterial != null)
        {
            platformMaterial.friction = customFriction;
        }
    }
}
