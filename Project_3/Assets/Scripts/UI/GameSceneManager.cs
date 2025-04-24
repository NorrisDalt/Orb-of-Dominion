using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
  public WaveSpawner wave;
  public int gameCleared = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Win();
    }

    public void Win()
    {
        if(wave.cleared == true)
        {
            SceneManager.LoadScene(2);
            gameCleared += 1;
            wave.cleared = false; 
        }

        if(gameCleared >= 3)
        {
            SceneManager.LoadScene("YouWin");
        }
    
    }
}
