using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f; // Kuglens hastighed
    public int damage = 10; // Hvor meget skade kuglen giver
    public float lifetime = 5f; // Hvor længe kuglen lever, før den destrueres

    private void Start()
    {
        // Ødelæg kuglen efter en vis tid, hvis den ikke rammer noget
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Flyt kuglen fremad baseret på dens hastighed
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tjek om kuglen rammer spilleren
        if (collision.gameObject.layer != 3)
        {

     
            
            if(collision.CompareTag("Player"))
            {
               //fjende spillerens liv
            }
            Destroy(gameObject);
        }
       
    }
}
