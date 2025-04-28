using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCon : MonoBehaviour
{
    public static WinCon Instance; // Singleton instance
    public static int count = 0;   // Persistent win counter

    void Awake()
    {
        // Singleton pattern: Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    void Update()
    {
        // Load win scene when count reaches 3
        if (count >= 3)
        {
            SceneManager.LoadScene("YouWin");
            ResetCounter(); // Optional: Reset after winning
        }
        //Debug.Log(count);
    }

    // Optional: Reset the counter (call manually if needed)
    public static void ResetCounter()
    {
        count = 0;
    }
}