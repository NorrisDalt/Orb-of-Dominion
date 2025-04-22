using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject topInstructionPanel;
    [SerializeField] private GameObject bottomInstructionPanel;
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI bottomText;

    public void ShowMovementInstructions()
    {
        ShowTop("Use WASD to move");
        HideBottom();
    }

    public void ShowOrbPickupInstructions()
    {
        ShowTop("Press Q to retrieve the orb");
        HideBottom();
    }

    public void ShowJumpInstructions()
    {
        ShowTop("Press Space to Jump");
        HideBottom();
    }

    public void ShowOrbPracticeInstructions()
    {
        ShowTop("Hit the target with the orb to proceed");
        ShowBottom("Press E to send the orb, F to recall it");
    }

    public void ShowCombatInstructions()
    {
        ShowTop("Practice your slash attack on the dummies");
        ShowBottom("Left Click to use slash attack (must have orb)");
    }

    public void OnOrbRetrieved()
    {
        ClearAllText();
        // Add door logic here if needed
    }

    public void OnTargetHit()
    {
        ClearAllText();
        // Add door logic here if needed
    }

    public void OnAllDummiesDestroyed()
    {
        ClearAllText();
        // Add final door logic here if needed
    }

    public void ClearAllText()
    {
        topText.text = "";
        bottomText.text = "";
        topInstructionPanel.SetActive(false);
        bottomInstructionPanel.SetActive(false);
    }

    private void ShowTop(string message)
    {
        topInstructionPanel.SetActive(true);
        topText.text = message;
    }

    private void ShowBottom(string message)
    {
        bottomInstructionPanel.SetActive(true);
        bottomText.text = message;
    }

    private void HideBottom()
    {
        bottomInstructionPanel.SetActive(false);
        bottomText.text = "";
    }
}
