using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Wave : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnDelay;
    public float waveDelay;

    public int GetEnemyCount(int waveIndex)
    {
        return enemyCount + (waveIndex * 2) + Random.Range(0, 3);
    }
}
