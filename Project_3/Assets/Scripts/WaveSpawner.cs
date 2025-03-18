using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Wave[] waves;
    public Transform[] spawnPoints;

    private int currentWaveIndex = 0;
    
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while(currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];

            for(int i = 0; i < currentWave.enemyCount; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // Sets spawnPoint to a random point from the spawnPoints array
                Instantiate(currentWave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);// Spawns at spawnPoint

                yield return new WaitForSeconds(currentWave.spawnDelay);
            }

            yield return new WaitForSeconds(currentWave.waveDelay);

            currentWaveIndex++;

        }
        Debug.Log("All waves completed!");
    }


   
}
