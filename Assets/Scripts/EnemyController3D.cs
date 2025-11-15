using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyController3D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 15f;
    public float attackRange = 2f;

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public int damage = 10;
    public float damageDelay = 0.4f; // wann im Angriff die Hitbox treffen soll

    private Transform target;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;
    private Ilumisoft.HealthSystem.Health health;


    private void Start()
    {
        animator = GetComponent<Animator>();

        // Spieler über Tag finden
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("EnemyController3D: Kein GameObject mit Tag 'Player' gefunden!");
        }

        // Health holen (vom Ilumisoft-System)
        health = GetComponent<Ilumisoft.HealthSystem.Health>();
        if (health != null)
        {
            health.OnHealthEmpty += OnDeath;

        }
        else
        {
            Debug.LogError("EnemyController3D: Kein Health Component gefunden!");
        }

    }

    private void Update()
    {
        if (isDead) return;
        if (target == null) return;

        // Richtung zum Spieler auf Bodenebene
        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
        {
            // zu weit weg -> Idle
            SetSpeed(0f);
            return;
        }

        if (distance > attackRange)
        {
            // Hinlaufen
            Vector3 moveDir = toPlayer.normalized;

            transform.position += moveDir * moveSpeed * Time.deltaTime;

            // Richtung drehen
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            SetSpeed(1f); // läuft
        }
        else
        {
            // In Angriffsreichweite
            SetSpeed(0f);
            TryAttack();
        }
    }

    void SetSpeed(float value)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", value); // Parameter im Animator
        }
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Trigger im Animator
        }

        // Schaden etwas verzögert, damit es zum Animations-Hit passt
        StartCoroutine(DealDamageAfterDelay(damageDelay));
    }

    IEnumerator DealDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target == null) yield break;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange + 0.5f)
        {
            // Player im Range -> Schaden zufügen über TinyHealthSystem
            if (HealthSystem.Instance != null)
            {
                Debug.Log($"Enemy hit the player for {damage} damage");
                HealthSystem.Instance.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("HealthSystem.Instance ist null – TinyHealthSystem nicht in Szene?");
            }
        }
    }


    private void OnDeath()
{
    if (isDead) return;
    isDead = true;

    Debug.Log("Zombie died!");

    // Bewegung stoppen
    SetSpeed(0f);

    // Death-Animation
    if (animator != null)
        animator.SetTrigger("Dead");

    // Hitbox deaktivieren, damit er keinen Schaden mehr kriegt
    var hitbox = GetComponent<Ilumisoft.HealthSystem.Hitbox>();
    if (hitbox != null)
        hitbox.enabled = false;

    // Rigidbody einfrieren, damit er nicht mehr fällt
    var rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    // WICHTIG: Collider NICHT deaktivieren,
    // damit er auf dem Boden "liegen" bleibt.
    // Collider col = GetComponent<Collider>();
    // if (col != null) col.enabled = false;   <-- WEG DAMIT

    // AI/Movement-Script ausschalten
    this.enabled = false;

    // Nach 2 Sekunden GameObject entfernen (oder Wert anpassen)
    Destroy(gameObject, 5f);
}


}
