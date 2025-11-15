using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    public float maxMana = 100f;
    public float currentMana = 100f;

    // optional: für Regeneration später
    public float manaRegenPerSecond = 0f;

    private void Update()
    {
        if (manaRegenPerSecond > 0f && currentMana < maxMana)
        {
            currentMana += manaRegenPerSecond * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    public bool TrySpend(float amount)
    {
        if (amount <= 0f) return true;

        if (currentMana < amount)
        {
            Debug.Log("Not enough mana!");
            return false;
        }

        currentMana -= amount;
        return true;
    }

    public void AddMana(float amount)
    {
        if (amount <= 0f) return;
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }
}
