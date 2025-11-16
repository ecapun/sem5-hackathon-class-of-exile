using UnityEngine;

[CreateAssetMenu(menuName = "Items/Stat Item")]
public class StatItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("Buffs")]
    public float healthManaRegen;   // gilt für HP + Mana
    public float bonusDamage;
    public float bonusMoveSpeed;
    public float bonusMaxHealth;
    public float bonusMaxMana;
}
