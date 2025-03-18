using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerDmg : MonoBehaviour
{
    public List<GameObject> allEnemiesList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //FindAllEnemies();
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            allEnemiesList.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    /*public void FindAllEnemies()
    {
       GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemiesInScene)
        {
            if(!allEnemiesList.Contains(enemy))
            {
                allEnemiesList.Add(enemy);
            }
        }

        Debug.Log("Total enemies added to list: " + allEnemiesList.Count);
    }*/
}
