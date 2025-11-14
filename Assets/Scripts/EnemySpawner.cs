using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Setup")]
    public GameObject enemyPrefab;

    [Header("Player Reference")]
    public Transform player;

    [Header("Spawn Distance")]
    public float minSpawnDistance = 25f;
    public float maxSpawnDistance = 35f;

    [Header("Waves")]
    public int startEnemies = 3;
    public int enemiesPerWaveIncrease = 2;
    public float timeBetweenWaves = 5f;
    public float spawnDelayBetweenEnemies = 0.2f;

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    void SpawnEnemyAroundPlayer()
    {
        if (player == null || enemyPrefab == null) return;

        // Zufällige Richtung auf Kreis um den Player (2D-Kreis auf XZ-Ebene)
        Vector2 circle = Random.insideUnitCircle.normalized;

        // Zufällige Distanz im Bereich [min, max]
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector3 spawnDir = new Vector3(circle.x, 0f, circle.y);
        Vector3 spawnPosition = player.position + spawnDir * distance;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    IEnumerator SpawnWaves()
    {
        // kleiner Startdelay
        yield return new WaitForSeconds(2f);

        while (true)
        {
            currentWave++;
            int enemyCount = startEnemies + (currentWave - 1) * enemiesPerWaveIncrease;

            Debug.Log($"Wave {currentWave} spawning {enemyCount} enemies.");

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemyAroundPlayer();
                yield return new WaitForSeconds(spawnDelayBetweenEnemies);
            }

            // warten, bis alle Gegner dieser Welle tot sind
            yield return new WaitUntil(() => FindObjectsOfType<EnemyHealth>().Length == 0);

            Debug.Log($"Wave {currentWave} cleared.");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}
