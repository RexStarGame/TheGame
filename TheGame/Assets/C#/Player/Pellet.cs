using UnityEngine;

public class Pellet : MonoBehaviour
{
    public float damage; // Skaden denne pellet giver
    public float lifetime; // Hvor længe denne pellet lever, hvis den ikke rammer noget

    void Start()
    {
        // Ødelæg pellet efter dens levetid
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Tjek for fjender med tag "Enemies"
        if (collision.collider.CompareTag("Enemies"))
        {
            EnemyHealthBar enemy = collision.gameObject.GetComponent<EnemyHealthBar>();
            if (enemy != null)
            {
                // Kalder fjendens TakeDamage-funktion og påfører skade
                enemy.TakeDamage(damage);
                
            }

            EnemyMovement enemyMovement = collision.gameObject.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.takeDamage(); // Fjenden reagerer på skaden
            }

            // Fjern pellet ved kollision med en fjende
            Destroy(gameObject);
        }

        // Tjek for bossen med tag "Boss"
        if (collision.collider.CompareTag("Boss"))
        {
            JulemandensHealth julemanden = collision.gameObject.GetComponent<JulemandensHealth>();
            if (julemanden != null)
            {
                julemanden.TagSkade((int)damage); // Påfør skade til julemanden
                
            }

            // Fjern pellet ved kollision med bossen
            Destroy(gameObject);
        }

        // Fjern pellet ved kollision uanset hvad der sker
        Destroy(gameObject);
    }
}
