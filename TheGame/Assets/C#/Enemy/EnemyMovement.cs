using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public Transform playerPos;
    public Transform currentPos; 
    public GameObject pointA;
    public GameObject pointB;
    public Rigidbody2D rb;
    public Animator animator;

    [SerializeField] private float speed;
    [SerializeField] private float viewDistance = 5f; // fjendes synafstand.
    [SerializeField] private float stopDistance = 5; // enemies stop moving whenever the player is in a certin range. 

    public LayerMask playerLayer; // lag til spilleren 

    private bool playerIsClose = false;
    private bool isUnderAttack = false; // tjækker om fjenden bliver angrebet af spillerern eller ej.
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPos = pointB.transform;
        animator.SetBool("isRunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        // tjek synsfelt og afstand til spilleren
        playerIsClose = IsPlayerInSight();
        if(isUnderAttack)
        {
            // vend mod spilleren
            FacePlayer();
            rb.velocity = Vector3.zero; // stop bevægelse
            animator.SetBool("isRunning", false);
            return;
        }
        if(playerPos != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerPos.position);
            playerIsClose = distanceToPlayer <= stopDistance;
        }
        if(playerIsClose)
        {
            rb.velocity = Vector2.zero; // stopper fjenden.
            animator.SetBool("isRunning", false);
            return;
        }
        animator.SetBool("isRuunning", true); //forsætter med at gå frem af 

        Vector2 point = currentPos.position;
       
        if(currentPos == pointB.transform) //
        {
            rb.velocity = new Vector2(speed, 0f); // right
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0f); // left
        }
        if (Vector2.Distance(transform.position, currentPos.position) < 0.5f && currentPos == pointB.transform) //moves to Point A if traget enter Point B.
        {
            flip();
            currentPos = pointA.transform;
        }
        if (Vector2.Distance(transform.position, currentPos.position) < 0.5f && currentPos == pointA.transform) // Moves to point B if traget enters Point A.
        {
            flip();
            currentPos = pointB.transform;
        }
    }
    public void FacePlayer()
    {
        if(player.position.x > transform.position.x && transform.localScale.x < 0 ||
            player.position.x < transform.position.x && transform.localScale.x > 0)
        {
            flip();
        }
    }
   
    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    private bool IsPlayerInSight()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance, playerLayer);

        if(hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true; 
        }

        return false;
    }
    public void takeDamage()
    {
        //Bliver kaldt når fjenden bliver angrbet!.

        isUnderAttack = true;
        animator.SetBool("isRunning", false);

        Invoke(nameof(ResetUnderAttack), 2f); // kalder reset isunderattack efter 2 secunder.
    }
    private void ResetUnderAttack()
    {
       isUnderAttack=false;
        //flip(); // flips turn around efter tid. 
        if ((currentPos == pointA.transform && transform.localScale.x > 0) ||
        (currentPos == pointB.transform && transform.localScale.x < 0))
        {
            flip(); // Flip kun, hvis fjenden ikke vender mod den korrekte retning
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance); // Stopafstand
        Gizmos.color = Color.blue;
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * viewDistance); // Synsfelt
    }
}
