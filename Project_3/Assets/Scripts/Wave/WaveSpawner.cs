using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public OrbMovement orb;
    public Wave[] waves;
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;

    private int currentWaveIndex = 0;
    public bool cleared = false;

    void Start()
    {
        orb = FindObjectOfType<OrbMovement>();
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log("Wave " + currentWaveIndex);

            int enemyCount = currentWave.GetEnemyCount(currentWaveIndex);

            for (int i = 0; i < enemyCount; i++)
            {
                if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
                {
                    Debug.LogError("Assign enemyPrefabs/spawnPoints in inspector!");
                    yield break;
                }

                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject clone = Instantiate(randomEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
                orb.allEnemiesList.Add(clone);

                yield return new WaitForSeconds(currentWave.spawnDelay);
                Debug.Log("Enemies in list: " + orb.allEnemiesList.Count);
            }

            while (orb.allEnemiesList.Count > 0)
            {
                orb.allEnemiesList.RemoveAll(enemy => enemy == null);
                yield return new WaitForSeconds(0.1f); // Safety delay
            }

            currentWaveIndex++;
        }

        Debug.Log("All waves cleared!");
        WinCon.count++;
        cleared = true;
    }
}