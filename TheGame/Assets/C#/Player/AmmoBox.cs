using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 20; // Mængden af ammo, som AmmoBoxen giver
    private bool isPlayerNearby = false; // Indikerer, om spilleren er tæt på
    private GameObject equippedWeapon; // Det aktive våben i spillerens hånd
    private PlayerShooting weaponShootingScript;

    void Update()
    {
        // Hvis spilleren er tæt nok på og trykker på "E"
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (equippedWeapon != null && weaponShootingScript != null)
            {
                weaponShootingScript.AddAmmo(ammoAmount);
                Debug.Log("Ammo added to weapon: " + equippedWeapon.name);
                Destroy(gameObject); // Fjern AmmoBoxen
            }
            else
            {
                Debug.LogWarning("No equipped weapon found or weapon has no shooting script.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Spilleren træder ind i collideren
        {
            isPlayerNearby = true;

            // Find våbnet i spillerens hånd
            SkiftVåbenFraInventory weaponManager = collision.GetComponent<SkiftVåbenFraInventory>();
            if (weaponManager != null)
            {
                equippedWeapon = weaponManager.GetEquippedWeapon(); // Hent det aktive våben
                if (equippedWeapon != null)
                {
                    weaponShootingScript = equippedWeapon.GetComponent<PlayerShooting>();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Spilleren forlader collideren
        {
            isPlayerNearby = false;
            equippedWeapon = null;
            weaponShootingScript = null;
        }
    }
}
