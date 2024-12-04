using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Kuglens hastighed
    [SerializeField] private int damage = 10; // Hvor meget skade kuglen giver
    [SerializeField] private float lifetime = 5f; // Hvor længe kuglen lever, før den destrueres

    private PlayerHealth health;
    private Vector3 direction;
    public void intallized(Vector3 firedirection)
    {
        direction = firedirection.normalized;
    }

    private void Start()
    {
        // Ødelæg kuglen efter en vis tid, hvis den ikke rammer noget
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Flyt kuglen fremad baseret på dens hastighed
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tjek om kuglen rammer spilleren
        if (collision.gameObject.layer != 3)
        {  
            if(collision.CompareTag("Player"))
            {
              health.TakeDamage(5);
            }
            Destroy(gameObject);
        }
       
    }
}
