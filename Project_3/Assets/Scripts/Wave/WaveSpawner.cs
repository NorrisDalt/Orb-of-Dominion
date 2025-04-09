using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public OrbMovement orb;
    public Wave[] waves;
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs; // Add this array for different enemy types

    private int currentWaveIndex = 0;
    public bool cleared = false;
    
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while(currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log("Wave " + currentWaveIndex);

            int enemyCount = currentWave.GetEnemyCount(currentWaveIndex);

            for(int i = 0; i < enemyCount; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                
                // Randomly select an enemy prefab from the array
                GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject clone = Instantiate(randomEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
                
                orb.allEnemiesList.Add(clone);

                yield return new WaitForSeconds(currentWave.spawnDelay);
                Debug.Log("Total enemies added to list: " + orb.allEnemiesList.Count);
            }

            while(orb.allEnemiesList.Count > 0)
            {
                orb.allEnemiesList.RemoveAll(enemy => enemy == null);
                yield return null;
            }

            currentWaveIndex++;
        }
        Debug.Log("All waves completed!");
        cleared = true;
    }
}