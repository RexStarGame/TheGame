using UnityEngine;

public class PickUpGuns : MonoBehaviour
{
    public float pickUpRange = 2f;
    public KeyCode pickUpKey = KeyCode.E;
    public Inventory playerInventory;
    private InventoryUI inventoryUI;
    private GameObject closestWeapon;
    public bool updatedInventroy = false;
    private PickUpGuns pickUpGuns; // Reference til PickUpGuns-scriptet
    void Start()
    {
        updatedInventroy = false; // Sørg for at starte som false
        pickUpGuns = FindObjectOfType<PickUpGuns>();

        if (playerInventory == null)
        {
            // Finder spilleren baseret på tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }

            if (playerInventory == null)
            {
                //Debug.LogError("Inventory kunne ikke findes på spilleren! Sørg for, at det er tildelt.");
                return;
            }
        }
    }


    void Update()
    {
        FindClosestWeapon(); // Find det nærmeste våben
        if (closestWeapon != null)
        {
            // Tjek om spilleren trykker på E for at samle op
            if (Input.GetKeyDown(pickUpKey))
            {
                TryPickUpWeapon(closestWeapon);
            }
        }
    }

    void FindClosestWeapon()
    {
        closestWeapon = null;
        float shortestDist = Mathf.Infinity;
        GameObject[] allWeapons = GameObject.FindGameObjectsWithTag("Weapon");
        // Sørg for at dine våben i verden har tag "Weapon"

        foreach (GameObject wpn in allWeapons)
        {
            float dist = Vector2.Distance(transform.position, wpn.transform.position);
            if (dist < shortestDist && dist < pickUpRange)
            {
                shortestDist = dist;
                closestWeapon = wpn;
            }
        }
    }

    void TryPickUpWeapon(GameObject weaponGO)
    {
        Item itemData = weaponGO.GetComponent<Item>();
        if (itemData != null)
        {
            // Check if itemID is already in inventory
            if (playerInventory.itemIDs.Contains(itemData.itemID))
            {
                //Debug.Log("Weapon is already in inventory. Cannot pick up again.");
                return;
            }

            if (playerInventory.AddItem(itemData.itemID))
            {
                // Marker våbnet som samlet op
                ShotgunShooting shotgunScript = weaponGO.GetComponent<ShotgunShooting>();
                if (shotgunScript != null)
                {
                    // Find UI-container dynamisk
                    GameObject ammoUI = GameObject.Find("AmmoUIContainer"); // Sørg for at navnet matcher i scenen
                    shotgunScript.InitializeUI(ammoUI);

                    // Marker våbnet som samlet op
                    shotgunScript.MarkAsPickedUp();
                }
                HealthBoxUI healthBoxUI = weaponGO.GetComponent<HealthBoxUI>();
                if (healthBoxUI != null)
                {
                    Debug.Log($"Skjuler UI for {weaponGO.name}");
                    //healthBoxUI.MarkAsPicked();
                }
                // Fjern objektet fra verden
                Destroy(weaponGO);
                //Debug.Log("Weapon picked up and removed from the world: " + weaponGO.name);

                // Marker inventory som opdateret
                updatedInventroy = true;

                // Opdater Inventory UI
                if (inventoryUI != null)
                {
                    inventoryUI.RefreshInventoryUI();
                }
                else
                {
                    //Debug.LogWarning("Could not find InventoryUI in the scene!");
                }
            }
        }
    }
}
