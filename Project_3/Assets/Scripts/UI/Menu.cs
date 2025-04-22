using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuParent;
    public GameObject mainCreditsParent;
    public GameObject mainControlsParent;
    //public GameObject creditsMain;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadPlayGame()
    {
        SceneManager.LoadScene("tutoriallevel");
    }

    public void LoadQuitGame()
    {
        Application.Quit();
    }

    public void OpenCreditsMain()
    {
        mainMenuParent.SetActive(false);
        mainCreditsParent.SetActive(true);
    }

    public void CloseCreditsMain()
    {
        mainMenuParent.SetActive(true);
        mainCreditsParent.SetActive(false);
    }

    public void OpenControlsMain()
    {
        mainMenuParent.SetActive(false);
        mainControlsParent.SetActive(true);
    }

    public void CloseControlsMain()
    {
        mainMenuParent.SetActive(true);
        mainControlsParent.SetActive(false);
    }
}