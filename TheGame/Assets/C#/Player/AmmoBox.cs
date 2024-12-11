using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 20; // M�ngden af ammo, som AmmoBoxen giver
    private bool isPlayerNearby = false; // Indikerer, om spilleren er t�t p�
    private GameObject equippedWeapon; // Det aktive v�ben i spillerens h�nd
    private PlayerShooting weaponShootingScript;

    void Update()
    {
        // Hvis spilleren er t�t nok p� og trykker p� "E"
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
        if (collision.CompareTag("Player")) // Spilleren tr�der ind i collideren
        {
            isPlayerNearby = true;

            // Find v�bnet i spillerens h�nd
            SkiftV�benFraInventory weaponManager = collision.GetComponent<SkiftV�benFraInventory>();
            if (weaponManager != null)
            {
                equippedWeapon = weaponManager.GetEquippedWeapon(); // Hent det aktive v�ben
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
