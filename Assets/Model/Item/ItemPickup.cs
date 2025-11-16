using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public StatItem itemData;

    private void Start()
    {
        Debug.Log("[ItemPickup] Aktiv auf: " + name);
    }
}
