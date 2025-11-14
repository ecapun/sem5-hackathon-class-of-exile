using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    public float deathDelay = 3f;

    private int currentHealth;
    private Animator animator;
    private bool isDead = false;

    public GameObject[] lootPrefabs; // im Inspector füllen
    public float lootDropChance = 0.5f; // 50%

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        // Optional: Hit-Reaction Animation hier triggern
    }

    

void Die()
{
    isDead = true;

    if (animator != null)
        animator.SetTrigger("Dead");

    var controller = GetComponent<EnemyController3D>();
    if (controller != null) controller.enabled = false;

    var col = GetComponent<Collider>();
    if (col != null) col.enabled = false;

    TrySpawnLoot();

    Destroy(gameObject, deathDelay);
}

void TrySpawnLoot()
{
    if (lootPrefabs == null || lootPrefabs.Length == 0) return;

    if (Random.value <= lootDropChance)
    {
        int index = Random.Range(0, lootPrefabs.Length);
        Instantiate(lootPrefabs[index], transform.position, Quaternion.identity);
    }
}

}
