using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private PlayerInventory inventory;

    private void Awake()
    {
        inventory = GetComponentInParent<PlayerInventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[ItemCollector] Trigger mit: " + other.name);

        var pickup = other.GetComponent<ItemPickup>();
        if (pickup == null)
            return;

        if (inventory == null)
        {
            Debug.LogWarning("[ItemCollector] Kein PlayerInventory gefunden");
            return;
        }

        if (pickup.itemData == null)
        {
            Debug.LogWarning("[ItemCollector] itemData ist null");
            return;
        }

        inventory.AddItem(pickup.itemData);
        Debug.Log("[ItemCollector] Item aufgenommen: " + pickup.itemData.itemName);

        Destroy(pickup.gameObject);
    }
}
