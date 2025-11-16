using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Gesammelte Items (nur Debug)")]
    public List<StatItem> items = new List<StatItem>();

    public float totalBonusDamage { get; private set; }

    private AnimationStateController animCtrl;

    private void Awake()
    {
        animCtrl = GetComponent<AnimationStateController>();
    }

    public void AddItem(StatItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Inventory] Item ist null");
            return;
        }

        items.Add(item);
        Debug.Log("[Inventory] Item aufgenommen: " + item.itemName);

        ApplyItemBuffs(item);
    }

    private void ApplyItemBuffs(StatItem item)
    {
        // 1) Health + Mana + Regen über HealthSystem-Singleton
        var hs = HealthSystem.Instance;
        if (hs != null)
        {
            // Regen-Buff (HP+Mana)
            if (item.healthManaRegen != 0f)
            {
                hs.regen += item.healthManaRegen;
                Debug.Log("[Inventory] Regen +" + item.healthManaRegen + " → jetzt " + hs.regen);
            }

            // Max HP: Wert ist in Prozent (50 = +50% MaxHP)
            if (item.bonusMaxHealth != 0f)
            {
                hs.SetMaxHealth(item.bonusMaxHealth);
                // einmal vollheilen, damit die Leiste nicht kleiner wird
                hs.HealDamage(99999f);
                Debug.Log("[Inventory] MaxHealth +" + item.bonusMaxHealth + "%");
            }

            // Max Mana (auch Prozent)
            if (item.bonusMaxMana != 0f)
            {
                hs.SetMaxMana(item.bonusMaxMana);
                hs.RestoreMana(99999f);
                Debug.Log("[Inventory] MaxMana +" + item.bonusMaxMana + "%");
            }
        }
        else
        {
            Debug.LogWarning("[Inventory] HealthSystem.Instance ist null – ist das HealthSystem in der Szene?");
        }

        // 2) Movement-Speed Buff
        if (animCtrl != null && item.bonusMoveSpeed != 0f)
        {
            animCtrl.moveSpeed += item.bonusMoveSpeed;
            Debug.Log("[Inventory] MoveSpeed +" + item.bonusMoveSpeed +
                      " → jetzt " + animCtrl.moveSpeed);
        }

        // 3) Damage merken (wird später bei Skills verwendet)
        if (item.bonusDamage != 0f)
        {
            totalBonusDamage += item.bonusDamage;
            Debug.Log("[Inventory] Total BonusDamage: " + totalBonusDamage);
        }
    }
}
