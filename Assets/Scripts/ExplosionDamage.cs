using UnityEngine;
using Ilumisoft.HealthSystem;

public class ExplosionDamage : MonoBehaviour
{
    [Header("Impact Hit (direkter Aufprall)")]
    public bool doImpactDamage = true;
    public float impactDelay = 0.0f;   // z.B. 0, wenn Prefab genau beim Einschlag gespawnt wird
    public float impactRadius = 2f;
    public float impactDamage = 30f;

    [Header("Späte Explosion")]
    public bool doExplosionDamage = true;
    public float explosionDelay = 2.0f; // Zeit NACH dem Impact
    public float explosionRadius = 4f;
    public float explosionDamage = 50f;

    private void Start()
    {
        StartCoroutine(DamageRoutine());
    }

    private System.Collections.IEnumerator DamageRoutine()
    {
        // 1) Impact-Schaden
        if (doImpactDamage)
        {
            if (impactDelay > 0f)
                yield return new WaitForSeconds(impactDelay);

            DoDamage(impactRadius, impactDamage, "[Impact]");
        }

        // 2) Späte Explosion
        if (doExplosionDamage && explosionDelay > 0f)
        {
            yield return new WaitForSeconds(explosionDelay);
            DoDamage(explosionRadius, explosionDamage, "[Explosion]");
        }
    }

    private void DoDamage(float radius, float damage, string label)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        Debug.Log($"[ExplosionDamage] {label} at {transform.position} hit {hits.Length} colliders (r={radius}).");

        foreach (var col in hits)
        {
            Hitbox hitbox = col.GetComponentInParent<Hitbox>();
            if (hitbox != null)
            {
                Debug.Log($"[ExplosionDamage] {label} hitting {hitbox.name} for {damage} damage.");
                hitbox.ApplyDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
