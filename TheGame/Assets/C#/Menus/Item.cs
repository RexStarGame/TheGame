using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemID; // Unikt ID for dette item

    // Valgfrit: Metode til at samle item op (kan tilpasses)
    public void PickUp()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null && inventory.AddItem(itemID))
        {
            gameObject.SetActive(false);
            //Debug.Log("Picked up item ID: " + itemID);
        }
        else
        {
            //Debug.Log("Cannot pick up item. Inventory might be full.");
        }
    }
}