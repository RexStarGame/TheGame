using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab for kuglen.
    public Transform shootPoint; // Punkt, hvor kuglen affyres fra.
    public float bulletSpeed = 20f; // Hastighed for kuglen.
    public int currentBulletAmount = 100; // Aktuelle antal skud.
    public int maxBulletAmount = 100; // Maksimum antal skud.
    public float reloadTime = 1f; // Tid til genladning.
    private bool isReloading = false;
    public int collectedAmmo = 0; // Samlet ammunition, som spilleren har opsamlet.

    public TextMeshProUGUI ammoText; // UI-tekst, der viser ammunitionen
    public GameObject reloadPrompt; // UI-element, der viser "Tryk R for at genlade"
    void Start()
    {
        // Opdater ammunitionsteksten ved start
        UpdateAmmoText();

        // Sørg for, at reloadPrompt er deaktiveret ved start
        if (reloadPrompt != null)
        {
            reloadPrompt.SetActive(false);
        }
    }
    void Update()
    {
        // Skyd, hvis spilleren trykker på "Fire1" og har kugler tilbage.
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.F)) && currentBulletAmount > 0 && !isReloading)
        {
            Shoot();
        }
        else if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.F)) && currentBulletAmount <= 0 && !isReloading)
        {
            // Spilleren har ingen kugler i magasinet
            if (collectedAmmo > 0)
            {
                // Vis "Tryk R for at genlade"
                if (reloadPrompt != null)
                {
                    reloadPrompt.SetActive(true);
                }
            }
            else
            {
                // Spilleren har ingen ammunition overhovedet
                // Du kan vise en anden besked her, hvis du ønsker det
            }
        }

        // Genlad, hvis spilleren trykker på "R" og har opsamlet ammo.
        if (Input.GetKeyDown(KeyCode.R) && currentBulletAmount < maxBulletAmount && collectedAmmo > 0 && !isReloading)
        {
            StartCoroutine(Reload());

            // Deaktiver reloadPrompt, da spilleren nu genlader
            if (reloadPrompt != null)
            {
                reloadPrompt.SetActive(false);
            }
        }
    }


    void Shoot()
    {
        // Reducer kugler fra magasinet.
        currentBulletAmount--;

        // Opdater ammunitionsteksten
        UpdateAmmoText();

        // Instantiér kuglen.
        if (bulletPrefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            // Tilføj hastighed til kuglen.
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootPoint.right * bulletSpeed * Mathf.Sign(transform.lossyScale.x);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Genlader...");
        yield return new WaitForSeconds(reloadTime);

        // Beregn hvor meget ammo der skal genlades
        int ammoNeeded = maxBulletAmount - currentBulletAmount;
        int ammoToReload = Mathf.Min(ammoNeeded, collectedAmmo);

        // Opdater ammunition
        currentBulletAmount += ammoToReload;
        collectedAmmo -= ammoToReload;

        // Opdater ammunitionsteksten
        UpdateAmmoText();

        isReloading = false;
        Debug.Log("Genladning færdig! Ammunition i magasinet: " + currentBulletAmount);
    }

    public void AddAmmo(int amount)
    {
        collectedAmmo += amount;
        Debug.Log("Samlet ammunition: " + collectedAmmo);

        // Opdater ammunitionsteksten
        UpdateAmmoText();
    }
    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = currentBulletAmount + " / " + collectedAmmo;
        }
    }
}