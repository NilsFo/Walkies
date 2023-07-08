using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool alreadyInteractedWith;

    public InteractableType myType;
    private GameState _gameState;
    public UnityEvent onInteractedWith;

    public enum InteractableType
    {
        Unknown,
        Pet,
        Treat,
        Tree,
        Friend
    }

    // Start is called before the first frame update
    void Start()
    {
        alreadyInteractedWith = false;
        if (onInteractedWith != null)
        {
            onInteractedWith = new UnityEvent();
        }

        if (myType == InteractableType.Unknown)
        {
            Debug.LogError("THIS GAME OBJECT IS DEFAULT INTERACTABLE", gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnInteractedWith()
    {
        alreadyInteractedWith = true;
        onInteractedWith.Invoke();
    }

    public bool IsInteractable()
    {
        return alreadyInteractedWith == false;
    }
}