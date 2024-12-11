using System.Collections;
using TMPro;
using UnityEngine;

public class ShotgunShooting : MonoBehaviour
{
    [Header("Shotgun Settings")]
    public GameObject pelletPrefab; // Prefab for hvert skud
    public Transform shootPoint; // Punkt hvor skud affyres fra
    public int pelletCount = 8; // Antal kugler pr. skud
    public float spreadAngle = 30f; // Spredningsvinkel
    public float pelletSpeed = 20f; // Hastighed p� kuglerne
    public float pelletLifetime = 2f; // Tid, f�r kuglen forsvinder

    [Header("Ammo Settings")]
    public int currentAmmo = 5; // Aktuelt antal skud i magasinet
    public int maxAmmo = 5; // Maksimalt antal skud i magasinet
    public int collectedAmmo = 10; // Ammunition i spillerens lager
    public float reloadTime = 2f; // Tid for genladning
    private bool isReloading = false;
    [Header("Animator")]
    public Animator animator;
    public float animationDelay = 0f; // Animation starter straks
    public float shootDelay = 0.3f;   // Skuddet forsinkes med 0,3 sekunder
    private bool canShoot = true; // Tjek om spilleren kan skyde
private float shootCooldown = 1f; // Cooldown-tid i sekunder
    [Header("Damage Settings")]
    public float maxDamage = 100f; // Maksimal skade fordelt mellem pellets

    [Header("UI Settings")]
    public GameObject reloadPrompt; // UI-element, der viser "Tryk R for at genlade"
    public TextMeshProUGUI ammoText; // UI-element der viser ammo status
    public GameObject ammoUIContainer; // Container til UI (sikrer nem visning/skjuling)

    private bool isPickedUp = false; // Om v�bnet er blevet samlet op
    private bool isEquipped = false; // Om v�bnet er udstyret
    void Start()
    {
        if (UIManager.Instance != null)
        {
            ammoUIContainer = UIManager.Instance.ammoUIContainer;
            ammoText = UIManager.Instance.ammoText;
            reloadPrompt = UIManager.Instance.reloadPrompt; // Hent reloadPrompt fra UIManager
        }
        if (reloadPrompt != null)
        {
            reloadPrompt.SetActive(false);
        }
        // Skjul UI, hvis v�bnet ikke er samlet op
        if (ammoUIContainer != null)
        {
            ammoUIContainer.SetActive(false);
        }

        UpdateAmmoUI();
    }
    public void AssignUIElements(GameObject uiContainer, TextMeshProUGUI uiText)
    {
        ammoUIContainer = uiContainer;
        ammoText = uiText;

        if (ammoUIContainer != null)
        {
            ammoUIContainer.SetActive(false); // Skjul UI ved start
        }
    }
    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;

        if (ammoUIContainer != null)
        {
            ammoUIContainer.SetActive(equipped); // Vis/skjul UI baseret p� om v�bnet er udstyret
        }

        if (equipped)
        {
            UpdateAmmoUI(); // Opdater UI, n�r v�bnet bliver udstyret
        }
    }
    void Update()
    {
        if (!isPickedUp || !isEquipped)
        {
            // S�rg for, at UI er skjult, hvis v�bnet ikke er samlet op eller udstyret
            if (ammoUIContainer != null && ammoUIContainer.activeSelf)
            {
                ammoUIContainer.SetActive(false);
            }
            if (reloadPrompt != null && reloadPrompt.activeSelf)
            {
                reloadPrompt.SetActive(false);
            }
            return;
        }

        // S�rg for, at UI er synligt, n�r shotgun er udstyret
        if (ammoUIContainer != null && !ammoUIContainer.activeSelf)
        {
            ammoUIContainer.SetActive(true);
        }
        if (reloadPrompt != null)
        {
            reloadPrompt.SetActive(currentAmmo <= 0 && collectedAmmo > 0 && !isReloading);
        }
        // Skyd, hvis spilleren trykker p� skyd-knappen og har ammo tilbage
        if (Input.GetButtonDown("Fire1") && canShoot && currentAmmo > 0 && !isReloading && PlayerHealth.isDead == false)
        {
            Animation();
        }
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && collectedAmmo > 0 && !isReloading)
        {
            StartCoroutine(Reload());

            // Deaktiver reloadPrompt, da spilleren nu genlader
            if (reloadPrompt != null)
            {
                reloadPrompt.SetActive(false);
            }
        }
        // Genlad, hvis spilleren trykker p� "R"
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && collectedAmmo > 0 && !isReloading)
        {
            StartCoroutine(Reload());
        } //siker at spilleren reloader automatisk hvis ikke han reloader n�r der er 0 skud tilbage i realod og stadigv�k pr�ver p� at sykde. 
    }
    public void MarkAsPickedUp()
    {
        isPickedUp = true;
    }
    void Animation()
    {
        if (animator != null)
        {
            // Start en coroutine for at sikre, at skud og animation sker synkront
            StartCoroutine(PlayShootAnimation());
        }
      
    }
    IEnumerator PlayShootAnimation()
    {
        if (!canShoot) yield break; // Afbryd, hvis spilleren er i cooldown

        // Aktiv�r cooldown
        canShoot = false;
        // Tilf�j en ventetid f�r animationen starter (valgfri)
        //float animationDelay = 0.1f; // Ventetid i sekunder f�r animationen starter
        yield return new WaitForSeconds(animationDelay);

        // S�rg for, at animationen afspilles
        animator.ResetTrigger("IsShooting");
        animator.SetTrigger("IsShooting");

        // Tilf�j en ventetid f�r skuddet udf�res
        //float shootDelay = 0.2f; // Ventetid i sekunder f�r skuddet udf�res
        yield return new WaitForSeconds(shootDelay);

        // Kald Shoot for at udf�re skuddet
        Shoot();
        // Vent til cooldown-tiden er g�et
        yield return new WaitForSeconds(shootCooldown);

        // Tillad spilleren at skyde igen
        canShoot = true;
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
            return;

        currentAmmo--; // Reduc�r ammo
      
        // Retning baseret p� spillerens orientering
        Vector2 shootDirection = shootPoint.right * transform.lossyScale.x; // Retning p�virket af spillerens skala (venstre/h�jre)
      
        // Skyd flere pellets
        
        for (int i = 0; i < pelletCount; i++)
        {
            // Beregn spredningsvinkel
            float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);

            // Kombiner spillerens retning med spredningsvinklen
            Vector3 finalDirection = spreadRotation * shootDirection;

            // Instanti�r pellet
            GameObject pellet = Instantiate(pelletPrefab, shootPoint.position, Quaternion.identity);

            // Tilf�j hastighed til kuglen
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = finalDirection.normalized * pelletSpeed; // Brug den kombinerede retning
            }

            // Tilf�j en komponent til skadeberegning
            Pellet pelletScript = pellet.AddComponent<Pellet>();
            pelletScript.damage = maxDamage / pelletCount;
            pelletScript.lifetime = pelletLifetime;

            // Fjern kuglen efter tid, hvis den ikke rammer noget
            Destroy(pellet, pelletLifetime);
        }
 

        UpdateAmmoUI(); // Opdater UI efter skud
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        // Beregn hvor meget ammo der skal genlades
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, collectedAmmo);

        // Opdater ammo
        currentAmmo += ammoToReload;
        collectedAmmo -= ammoToReload;

        isReloading = false;
        UpdateAmmoUI(); // Opdater UI efter genladning
    }
    public void StopReload()
    {
        isReloading = false;
        StopAllCoroutines();
    }
    public void ResumeReload()
    {
        if(currentAmmo > 0)
        {
            isReloading = false;
            return;
        }

        if (currentAmmo < maxAmmo && collectedAmmo < 0)
        {
            StartCoroutine(Reload());
        }
    }
    public void AddAmmo(int amount)
    {
        collectedAmmo += amount;
        //Debug.Log("Samlet ammunition: " + collectedAmmo);

        // Opdater ammunitionsteksten
        UpdateAmmoUI();
    }
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {collectedAmmo}";
        }
    }
    public void InitializeUI(GameObject uiContainer)
    {
        if (uiContainer != null)
        {
            ammoUIContainer = uiContainer;
            ammoUIContainer.SetActive(false); // Skjul UI ved start
        }
    }
    bool IsEquipped()
    {
        SkiftV�benFraInventory weaponManager = FindObjectOfType<SkiftV�benFraInventory>();
        if (weaponManager != null)
        {

            // Tilf�j kontrol for, om v�bnet er samlet op
            if (!isPickedUp)
            {
                return false;
            }

            return weaponManager.GetEquippedWeapon() == gameObject;
        }
        return false;
    }
}