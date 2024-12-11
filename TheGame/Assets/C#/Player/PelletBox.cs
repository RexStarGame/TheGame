using UnityEngine;

public class PelletBox : MonoBehaviour
{
    public int pelletAmount = 10; // Antal pellets som ammo boksen indeholder.
    private bool isPlayerNear = false; // Checker om spilleren er tæt nok på.
    private GameObject spillerensHånd; // Det aktive våben i spillerens hånd.
    private ShotgunShooting shotgunShooting; // Reference til ShotgunShooting.

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (spillerensHånd != null && shotgunShooting != null && pelletAmount > 0)
            {
                // Tilføj pellets til våbnet
                shotgunShooting.AddAmmo(pelletAmount);

                // Ødelæg boksen, da den er blevet brugt
                Destroy(gameObject);

                Debug.Log("Ammo box collected and destroyed.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;

            // Finder våbnet i spillerens hånd.
            SkiftVåbenFraInventory weaponManager = collision.GetComponent<SkiftVåbenFraInventory>();
            if (weaponManager != null)
            {
                spillerensHånd = weaponManager.GetEquippedWeapon();
                if (spillerensHånd != null)
                {
                    shotgunShooting = spillerensHånd.GetComponent<ShotgunShooting>();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            spillerensHånd = null;
            shotgunShooting = null;
        }
    }
}
