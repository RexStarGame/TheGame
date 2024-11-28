using System.Collections;
using System.Collections.Generic;
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

    void Update()
    {
        // Skyd, hvis spilleren trykker p� "Fire1" og har kugler tilbage.
        if (Input.GetButtonDown("Fire1") && currentBulletAmount > 0 && !isReloading)
        {
            Shoot();
        }

        // Genlad, hvis spilleren trykker p� "R".
        if (Input.GetKeyDown(KeyCode.R) && currentBulletAmount < maxBulletAmount)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        // Reducer kugler fra spilleren.
        currentBulletAmount--;

        // Instanti�r kuglen.
        if (bulletPrefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            // Tilf�j hastighed til kuglen.
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootPoint.right * bulletSpeed;
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true; //tj�kker om reload er sandt.
        Debug.Log("Genlader...");
        yield return new WaitForSeconds(reloadTime);

        // Overf�r samlet ammo til nuv�rende kugler.
        int ammoToReload = Mathf.Min(maxBulletAmount - currentBulletAmount);
        currentBulletAmount += ammoToReload;
        //collectedAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log("Genladning f�rdig!");
    }
}