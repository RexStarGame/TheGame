using UnityEngine;

public class PelletBox : MonoBehaviour
{
    public int pelletAmount = 10; // Antal pellets som ammo boksen indeholder.
    private bool isPlayerNear = false; // Checker om spilleren er t�t nok p�.
    private GameObject spillerensH�nd; // Det aktive v�ben i spillerens h�nd.
    private ShotgunShooting shotgunShooting; // Reference til ShotgunShooting.

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (spillerensH�nd != null && shotgunShooting != null && pelletAmount > 0)
            {
                // Tilf�j pellets til v�bnet
                shotgunShooting.AddAmmo(pelletAmount);

                // �del�g boksen, da den er blevet brugt
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

            // Finder v�bnet i spillerens h�nd.
            SkiftV�benFraInventory weaponManager = collision.GetComponent<SkiftV�benFraInventory>();
            if (weaponManager != null)
            {
                spillerensH�nd = weaponManager.GetEquippedWeapon();
                if (spillerensH�nd != null)
                {
                    shotgunShooting = spillerensH�nd.GetComponent<ShotgunShooting>();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            spillerensH�nd = null;
            shotgunShooting = null;
        }
    }
}
