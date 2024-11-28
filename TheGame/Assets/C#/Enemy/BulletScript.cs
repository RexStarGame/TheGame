using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage = 15f; // Skaden, kuglen g�r.
    public float lifetime = 5f; // Hvor l�nge kuglen eksisterer, hvis den ikke rammer noget.

    void Start()
    {
        // �del�g kuglen efter en vis tid, hvis den ikke rammer noget.
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D kaldt");
        Debug.Log($"Kugle ramte: {collision.gameObject.name}");

        // Tjek om kuglen rammer en fjende.
        EnemyHealthBar enemy = collision.gameObject.GetComponent<EnemyHealthBar>();
        if (enemy != null)
        {
            Debug.Log("EnemyHealthBar fundet p� objektet");
            // Kalder fjendens TakeDamage-funktion og p�f�rer skade.
            enemy.TakeDamage(damage);
            Debug.Log($"Fjende ramt! {damage} skade p�f�rt.");
        }
        else
        {
            Debug.Log("EnemyHealthBar IKKE fundet p� objektet");
        }

        // �del�g kuglen ved kollision, uanset hvad den rammer.
        Destroy(gameObject);
    }
}