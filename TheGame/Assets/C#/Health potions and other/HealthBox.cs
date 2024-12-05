using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : MonoBehaviour
{
    
    [Header("Healing Settings")]
    public int healAmount = 25; // Mængden af helbred, spilleren får
    public float pickupRange = 2f; // Afstand inden for hvilken spilleren kan samle objektet op

    private Transform player; // Referencen til spilleren

    void Start()
    {
        // Find spilleren i scenen (forudsat spilleren har tagget "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            //Debug.LogError("Ingen spiller fundet! Sørg for, at spilleren har tagget 'Player'.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Tjek om spilleren er inden for rækkevidde
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= pickupRange)
        {
            // Spilleren er tæt nok på, og vi lytter efter "E"-tasten
            if (Input.GetKeyDown(KeyCode.E))
            {
                HealPlayer();
            }
        }
    }

    void HealPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Begræns heal til MaxHealth
            int newHealth = Mathf.Min(playerHealth.CurrentHealth + healAmount, playerHealth.MaxHealth);
            playerHealth.CurrentHealth = newHealth;

            // Opdater slideren
            playerHealth.SetHealth(newHealth);

            //Debug.Log($"Spilleren er blevet helet med {healAmount} HP. Nuværende HP: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}");
        }
        else
        {
            //Debug.LogError("PlayerHealth-scriptet er ikke fundet på spilleren!");
        }

        // Fjern heal-objektet
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        // Tegn en visuel cirkel, der repræsenterer pickup-rækkevidden
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}

