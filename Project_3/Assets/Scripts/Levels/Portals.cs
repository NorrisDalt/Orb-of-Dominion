using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Portals : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject promptUI;

    [SerializeField] private InputActionReference interactAction; // ← assign "Interact" in the Inspector

    private bool playerIsNear = false;

    private void OnEnable()
    {
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact performed");
        
        if (playerIsNear)
        {
            LoadLevel();
        }
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}