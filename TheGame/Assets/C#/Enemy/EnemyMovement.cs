using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour
{

    public Transform currentPos; 
    public GameObject pointA;
    public GameObject pointB;
    public Rigidbody2D rb;
    public Animator animator;
    public float speed; 
  

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
    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    private void OnDrawGizmos()
    {
   
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f); // test
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}
