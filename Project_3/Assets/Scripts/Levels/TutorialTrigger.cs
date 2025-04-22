using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public enum TutorialStep
    {
        Move,
        OrbPickup,
        Jump,
        OrbPractice,
        Combat
    }

    [SerializeField] private TutorialStep step;

    private TutorialManager tutorialManager;

    private void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (step)
            {
                case TutorialStep.Move:
                    tutorialManager.ShowMovementInstructions();
                    break;
                case TutorialStep.OrbPickup:
                    tutorialManager.ShowOrbPickupInstructions();
                    break;
                case TutorialStep.Jump:
                    tutorialManager.ShowJumpInstructions();
                    break;
                case TutorialStep.OrbPractice:
                    tutorialManager.ShowOrbPracticeInstructions();
                    break;
                case TutorialStep.Combat:
                    tutorialManager.ShowCombatInstructions();
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.ClearAllText();
        }
    }
}
