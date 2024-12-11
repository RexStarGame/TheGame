using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage = 15f; // Skaden, kuglen gør.
    public float lifetime = 5f; // Hvor længe kuglen eksisterer, hvis den ikke rammer noget.

    void Start()
    {
        // Ødelæg kuglen efter en vis tid, hvis den ikke rammer noget.
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("OnCollisionEnter2D kaldt");
        //Debug.Log($"Kugle ramte: {collision.gameObject.name}");

        // Tjek om kuglen rammer en fjende.
        if (collision.collider.CompareTag("Enemies"))
        {
            EnemyHealthBar enemy = collision.gameObject.GetComponent<EnemyHealthBar>();
            if (enemy != null)
            {
                //Debug.Log("EnemyHealthBar fundet på objektet");
                // Kalder fjendens TakeDamage-funktion og påfører skade.
                enemy.TakeDamage(damage);

                //Debug.Log($"Fjende ramt! {damage} skade påført.");
            }

            EnemyMovement enemyMovement = collision.gameObject.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.takeDamage(); // Fjenden reagerer på skaden
            }
        }

        // Tjek om kuglen rammer julemanden (bossen)
        if (collision.collider.CompareTag("Boss"))
        {
            JulemandensHealth julemanden = collision.gameObject.GetComponent<JulemandensHealth>();
            if (julemanden != null)
            {
                julemanden.TagSkade((int)damage); // Påfør skade til julemanden
                Debug.Log($"Julemanden tog {damage} skade!");
            }
        }

        // Ødelæg kuglen ved kollision, uanset hvad den rammer.
        Destroy(gameObject);
    }
}
