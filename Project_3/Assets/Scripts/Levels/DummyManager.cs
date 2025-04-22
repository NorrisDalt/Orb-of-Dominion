using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyManager : MonoBehaviour
{
    [SerializeField] private int totalDummies = 4;
    [SerializeField] private GameObject doorToDestroy;

    private int destroyedCount = 0;

    private void OnEnable()
    {
        DummyEnemy.OnDummyDestroyed += HandleDummyDestroyed;
    }

    private void OnDisable()
    {
        DummyEnemy.OnDummyDestroyed -= HandleDummyDestroyed;
    }

    private void HandleDummyDestroyed()
    {
        destroyedCount++;

        if (destroyedCount >= totalDummies)
        {
            if (doorToDestroy != null)
                Destroy(doorToDestroy);

            FindObjectOfType<TutorialManager>()?.OnAllDummiesDestroyed();
        }
    }
}
