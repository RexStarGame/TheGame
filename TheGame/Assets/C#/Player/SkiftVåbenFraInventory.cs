using TMPro;
using UnityEngine;

public class SkiftVåbenFraInventory : MonoBehaviour
{
    public KeyCode switchKey = KeyCode.T; // Hvis du stadig vil have en tast til skift
    private Inventory playerInventory;
    private int currentWeaponIndex = 0;
    public Transform handTransform; // Sæt dette i Inspector til spillerens hånd
    public ItemManager itemManager; // Assign via Inspector

    private GameObject equippedWeapon;


    [Header("Animations")]

    public Animator animator;
    void Start()
    {
        animator.GetComponent<Animator>();
        playerInventory = GetComponent<Inventory>();
        if (playerInventory == null)
        {
            //Debug.LogError("No Inventory found on player!");
        }

        if (itemManager == null)
        {
            itemManager = FindObjectOfType<ItemManager>();
            if (itemManager == null)
            {
                //Debug.LogError("No ItemManager found in the scene. Please assign it in Inspector.");
            }
            else
            {
                itemManager.Initialize();
            }
        }

        EquipCurrentWeapon(); // Equipping the initial weapon
    }
   

    // Tilføj denne metode, så andre scripts kan få det aktive våben
    public GameObject GetEquippedWeapon()
    {
        return equippedWeapon;
    }
    void Update()
    {
        // Håndterer samling af items
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Eksempel på at samle et item op
            int newItemID = 1; // Dette skal være det faktiske itemID du samler op
            if (playerInventory.ItemCount < playerInventory.maxSize)
            {
                bool success = playerInventory.AddItem(newItemID);
                if (success)
                {
                    //Debug.Log("Item samlet op!");
                }
                else
                {
                    //Debug.LogWarning("Kunne ikke samle item op. Inventory er fuld.");
                }
            }
            else
            {
                //Debug.LogWarning("Inventory er fuld.");
            }
        }

        // Håndterer skift af våben via musens scrollhjul
        HandleWeaponSwitching();
    }

    void HandleWeaponSwitching()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            // Scroll op: skift til næste våben
            SwitchWeapon(1);
        }
        else if (scroll < 0f)
        {
            // Scroll ned: skift til forrige våben
            SwitchWeapon(-1);
        }
    }

    void SwitchWeapon(int direction)
    {
        if (playerInventory == null) return;

        // Gem den oprindelige index for reference
        int originalIndex = currentWeaponIndex;

        // Opdater index baseret på scrollretningen
        currentWeaponIndex += direction;

        // Wrap around logic
        if (currentWeaponIndex >= playerInventory.maxSize)
        {
            currentWeaponIndex = 0;
        }
        else if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = playerInventory.maxSize - 1;
        }

        //Debug.Log($"Skifter våben fra slot {originalIndex} til slot {currentWeaponIndex}");

        EquipCurrentWeapon();
    }
    GameObject FindExistingWeapon(int itemID)
    {
        // Gennemse alle børn af handTransform for at finde et våben med det ønskede itemID
        foreach (Transform child in handTransform)
        {
            Item item = child.GetComponent<Item>();
            if (item != null && item.itemID == itemID)
            {
                return child.gameObject;
            }
        }
        return null;
    }
    void EquipCurrentWeapon()
    {
        // Deaktiver det nuværende våben
        if (equippedWeapon != null)
        {
            var previousShotgun = equippedWeapon.GetComponent<ShotgunShooting>();
            if (previousShotgun != null)
            {
                previousShotgun.SetEquipped(false); // Skjul UI for tidligere våben
                previousShotgun.StopReload(); // Stop reload midlertidigt
            }

            var previousPistol = equippedWeapon.GetComponent<PlayerShooting>();
            if (previousPistol != null)
            {
                previousPistol.SetEquipped(false); // Skjul UI for tidligere våben
                previousPistol.StopReload(); // Stop reload midlertidigt
            }

            equippedWeapon.SetActive(false); // Deaktiver våbnet
        }

        int itemID = playerInventory.GetItemAtIndex(currentWeaponIndex);
        if (itemID == -1)
        {
            animator.SetBool("GrabedAGun", false); // Sørg for, at animationen deaktiveres
            return;
        }

        // Kontroller, om våbnet allerede er i hånden
        GameObject existingWeapon = FindExistingWeapon(itemID);
        if (existingWeapon != null)
        {
            equippedWeapon = existingWeapon;
            equippedWeapon.SetActive(true); // Aktivér våbnet igen
            animator.speed = 2f; // Hurtigere afspilningshastighed
            animator.SetTrigger("EquipWeapon");
            animator.SetBool("GrabedAGun", true); // Marker våbnet som værende i hånden
        }
        else
        {
            // Hent weapon prefab fra ItemManager
            GameObject weaponPrefab = itemManager.GetWeaponPrefabForItem(itemID);
            if (weaponPrefab == null)
            {
                animator.SetBool("GrabedAGun", false); // Sørg for, at animationen deaktiveres
                return;
            }

            // Instantiate nyt våben og tilknyt til handTransform
            equippedWeapon = Instantiate(weaponPrefab, handTransform);

            // Tjek om det er en shotgun og juster positionen
            if (equippedWeapon.GetComponent<ShotgunShooting>() != null)
            {
                equippedWeapon.transform.localPosition = new Vector3(0f, -0.218f, 0f); // Juster position for shotgun
            }
            else
            {
                equippedWeapon.transform.localPosition = Vector3.zero; // Standard position for andre våben
            }

            equippedWeapon.transform.localRotation = Quaternion.identity;
            equippedWeapon.SetActive(true);
            animator.speed = 1f; // Normal afspilningshastighed
            animator.SetTrigger("EquipWeapon");
            animator.SetBool("GrabedAGun", true);
        }
        animator.speed = 1f; // Sørg for, at efterfølgende animationer spilles normalt
        // Marker våbnet som aktivt i dets script
        var shotgunScript = equippedWeapon.GetComponent<ShotgunShooting>();
        if (shotgunScript != null)
        {
            shotgunScript.MarkAsPickedUp();

            // Find UI-elementerne fra scenen
            GameObject ammoUI = UIManager.Instance?.ammoUIContainer; // Brug UIManager til at finde container
            TextMeshProUGUI ammoText = UIManager.Instance?.ammoText;

            // Tildel UI-elementerne og marker våbnet som udstyret
            shotgunScript.AssignUIElements(ammoUI, ammoText);
            shotgunScript.SetEquipped(true);
            //shotgunScript.ResumeReload(); // Genoptag reload, hvis nødvendigt
        }

        var pistolScript = equippedWeapon.GetComponent<PlayerShooting>();
        if (pistolScript != null)
        {
            pistolScript.MarkAsPickedUp();

            // Find UI-elementerne fra scenen
            GameObject ammoUI = UIManager.Instance?.ammoUIContainer; // Brug UIManager til at finde container
            TextMeshProUGUI ammoText = UIManager.Instance?.ammoText;

            // Tildel UI-elementerne og marker våbnet som udstyret
            pistolScript.AssignUIElements(ammoUI, ammoText);
            pistolScript.SetEquipped(true);
            //pistolScript.ResumeReload(); // Genoptag reload, hvis nødvendigt
        }

        // Tilføj denne ekstra kontrol for at deaktivere animation, hvis intet våben blev valgt
        if (itemID == -1)
        {
            animator.SetBool("GrabedAGun", false);
            return;
        }
    }
}