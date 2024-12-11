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
    private int ammoBeingReloaded = 0; // Holder styr på ammo tilføjet under reload



    [Header("UI Settings")]

    public GameObject reloadPrompt; // UI-element, der viser "Tryk R for at genlade"

    public TextMeshProUGUI ammoText; // UI-tekst, der viser ammunitionen
    public GameObject ammoUIContainer; // Container til UI (sikrer nem visning/skjuling)

    private bool isPickedUp = false; // Om våbnet er blevet samlet op
    private bool isEquipped = false; // Om våbnet er udstyret

    [Header("Animator")]
    public Animator animator;
    void Start()
    {
        if (UIManager.Instance != null)
        {
            ammoUIContainer = UIManager.Instance.ammoUIContainer;
            ammoText = UIManager.Instance.ammoText;
            reloadPrompt = UIManager.Instance.reloadPrompt; // Hent reloadPrompt fra UIManager
        }

        // Sørg for, at reloadPrompt er deaktiveret ved start
        if (reloadPrompt != null)
        {
            reloadPrompt.SetActive(false);
        }
        // Opdater ammunitionsteksten ved start
        UpdateAmmoText();
    }
    void Update()
    {
        if (!isPickedUp || !isEquipped)
        {
            // Sørg for, at UI er skjult, hvis våbnet ikke er samlet op eller udstyret
            if (ammoUIContainer != null && ammoUIContainer.activeSelf)
            {
                ammoUIContainer.SetActive(false);
            }
            if (reloadPrompt != null && reloadPrompt.activeSelf)
            {
                reloadPrompt.SetActive(false); // Skjul reloadPrompt, hvis våbnet ikke er aktivt
            }

            return;
        }
        if (!isPickedUp || !isEquipped)
        {
            if (reloadPrompt != null && reloadPrompt.activeSelf)
            {
                reloadPrompt.SetActive(false); // Skjul reloadPrompt, hvis våbnet ikke er aktivt
            }
            return;
        }
        // Sørg for, at UI er synligt, når shotgun er udstyret
        if (ammoUIContainer != null && !ammoUIContainer.activeSelf)
        {
            ammoUIContainer.SetActive(true);
        }
        // Check for reloadPrompt
        if (reloadPrompt != null)
        {
            reloadPrompt.SetActive(currentBulletAmount <= 0 && collectedAmmo > 0 && !isReloading);
        }
        // Skyd, hvis spilleren trykker på "Fire1" og har kugler tilbage.
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.F)) && currentBulletAmount > 0 && !isReloading)
        {
            animator.SetTrigger("IsShoting");
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

    public void MarkAsPickedUp()
    {
        isPickedUp = true;
        Debug.Log($"{gameObject.name} er blevet samlet op.");
    }

    void Shoot()
    {
        // Reducer kugler fra magasinet.
        currentBulletAmount--;

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

        // Opdater ammunitionsteksten
        UpdateAmmoText();
    }

    IEnumerator Reload()
    {
        isReloading = true;
        ammoBeingReloaded = 0; // Nulstil midlertidig variabel
        //Debug.Log("Genlader...");
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
        //Debug.Log("Genladning færdig! Ammunition i magasinet: " + currentBulletAmount);
    }
    public void StopReload()
    {
        isReloading = false; // Sørg for, at reload stopper
        StopAllCoroutines(); // Stopper alle igangværende coroutines
        Debug.Log("Reload interrupted.");
    }
    public void ResumeReload()
    {
        // Hvis magasinet allerede har skud, lad spilleren skyde
        if (currentBulletAmount > 0)
        {
            Debug.Log("Magasinet har stadig skud. Ingen grund til at genlade.");
            isReloading = false; // Sørg for, at våbnet ikke er i reload-status
            return;
        }

        // Hvis magasinet er tomt og genladning er nødvendig
        if (currentBulletAmount < maxBulletAmount && collectedAmmo > 0)
        {
            Debug.Log("Resuming reload after weapon re-equip.");
            StartCoroutine(Reload());
        }
    }
    public void AddAmmo(int amount)
    {
        collectedAmmo += amount;
        //Debug.Log("Samlet ammunition: " + collectedAmmo);

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
    public void InitializeUI(GameObject uiContainer)
    {
        if (uiContainer != null)
        {
            ammoUIContainer = uiContainer;
            ammoUIContainer.SetActive(false); // Skjul UI ved start
        }
        else
        {
            Debug.LogWarning("UI Container not found or not assigned.");
        }
    }
    bool IsEquipped()
    {
        SkiftVåbenFraInventory weaponManager = FindObjectOfType<SkiftVåbenFraInventory>();
        if (weaponManager != null)
        {
            Debug.Log($"Equipped weapon: {weaponManager.GetEquippedWeapon()?.name}");

            // Tilføj kontrol for, om våbnet er samlet op
            if (!isPickedUp)
            {
                Debug.LogWarning("Våbnet er ikke samlet op.");
                return false;
            }

            return weaponManager.GetEquippedWeapon() == gameObject;
        }
        Debug.LogWarning("WeaponManager ikke fundet i scenen.");
        return false;
    }
    public void AssignUIElements(GameObject uiContainer, TextMeshProUGUI uiText)
    {
        ammoUIContainer = uiContainer;
        ammoText = uiText;

        if (ammoUIContainer != null)
        {
            ammoUIContainer.SetActive(false); // Skjul UI ved start
        }
        else
        {
            Debug.LogWarning("Ammo UI Container is not assigned or found!");
        }

        if (ammoText == null)
        {
            Debug.LogWarning("Ammo Text UI is not assigned or found!");
        }
    }
    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;

        if (ammoUIContainer != null)
        {
            ammoUIContainer.SetActive(equipped); // Vis/skjul UI baseret på om våbnet er udstyret
        }
        if (reloadPrompt != null)
        {
            reloadPrompt.SetActive(false); // Sørg for, at reloadPrompt er skjult, når våbnet ikke er udstyret
        }
        if (equipped)
        {
            UpdateAmmoText(); // Opdater UI, når våbnet bliver udstyret
        }
    }
}
