using UnityEngine;
using UnityEngine.UI; // Hvis du bruger UI-elementer

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 20; // Mængden af ammo, som AmmoBoxen giver
    private bool isPlayerNearby = false; // Indikerer om spilleren er tæt på

    // (Valgfrit) Reference til UI-tekst eller prompt
    // public GameObject pickupPrompt; // Træk din UI-prompt herind i Inspector

    private PlayerShooting playerShooting;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (playerShooting != null)
            {
                playerShooting.AddAmmo(ammoAmount);
                Debug.Log("Spilleren samlede en AmmoBox op og fik " + ammoAmount + " ammo.");

                // (Valgfrit) Deaktiver prompten
                // if (pickupPrompt != null)
                // {
                //     pickupPrompt.SetActive(false);
                // }

                Destroy(gameObject); // Fjern AmmoBoxen fra spillet
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerShooting = collision.GetComponent<PlayerShooting>();

            // (Valgfrit) Aktiver prompten
            // if (pickupPrompt != null)
            // {
            //     pickupPrompt.SetActive(true);
            // }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerShooting = null;

            // (Valgfrit) Deaktiver prompten
            // if (pickupPrompt != null)
            // {
            //     pickupPrompt.SetActive(false);
            // }
        }
    }
}
