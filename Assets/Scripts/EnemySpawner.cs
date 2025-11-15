using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Setup")]
    public GameObject enemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;   // feste Spawn-Punkte

    [Header("Spawn Settings")]
    public int spawnAmount = 5;       // wie viele sollen spawnen?

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < spawnAmount; i++)
        {
            // zufälligen Punkt auswählen
            int index = Random.Range(0, spawnPoints.Length);
            Transform p = spawnPoints[index];

            Instantiate(enemyPrefab, p.position, p.rotation);
        }
    }
}
