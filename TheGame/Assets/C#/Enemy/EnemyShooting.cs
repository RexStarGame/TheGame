using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject enemyBullet;
    public Transform firePoint;
    
    public int currentBulletAmount = 10;
    public int maxBulletAmount = 10;
    public float reloadTime = 2f;
    public float shootCooldown = 1f; // Tid mellem skud (sekunder).
    private bool isReloading = false;
    private float lastShotTime = 0f; // Tidspunkt for sidste skud.

   


    public float detectionRange = 20f; // Hvor langt fjenden kan "se" spilleren.
    public LayerMask playerLayer; // Lag, der indeholder spilleren.

    void Update()
    {
        if (IsPlayerInRange() && currentBulletAmount > 0 && !isReloading && Time.time > lastShotTime + shootCooldown)
        {
            Shoot();
        }
        else if (currentBulletAmount < maxBulletAmount && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    bool IsPlayerInRange()
    {
        // Tegn Raycast i Editor
        Debug.DrawRay(firePoint.position, firePoint.right * detectionRange * transform.localScale.x, Color.red);

        // Udfør Raycast med korrekt LayerMask
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, detectionRange * transform.localScale.x, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

    void Shoot()
    {
        //Debug.Log("Skyder!");
        currentBulletAmount--;
        lastShotTime = Time.time; // Opdaterer tidspunktet for sidste skud.

        if (enemyBullet != null && firePoint != null)
        {
            GameObject bullet = Instantiate(enemyBullet, firePoint.position, firePoint.rotation);

            Vector3 fireDirection = transform.localScale.x > 0 ? Vector3.right: Vector3.left;

            bullet.GetComponent<EnemyBullet>().intallized(fireDirection);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.right;
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        //Debug.Log("Genlader...");
        yield return new WaitForSeconds(reloadTime);

        currentBulletAmount = maxBulletAmount;
        isReloading = false;
        //Debug.Log("Genladning færdig!");
    }

    private void OnDrawGizmosSelected()
    {
        // Tegn en visuel repræsentation af detektionsområdet.
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * detectionRange);
    }
}
