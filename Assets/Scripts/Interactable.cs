using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool alreadyInteractedWith;

    public InteractableType myType;
    private GameState _gameState;
    public UnityEvent onInteractedWith;
    
    [Header("Interactions")]
    public float interactionSnapDistance = 1.0f;
    public bool showRange = false;

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
        _gameState = FindObjectOfType<GameState>();
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

        if (!_gameState.InteractionsCount.ContainsKey(myType))
        {
            _gameState.InteractionsCount.Add(myType, 0);
        }

        _gameState.InteractionsCount[myType] += 1;
        print("Interactions with '" + myType + "' is now: " + _gameState.InteractionsCount[myType]);
        
        // Play animation
        _gameState.player.PlayLockedAnimation(GetCurrentSnapPoint(),transform.position);
    }

    public bool IsInteractable()
    {
        return alreadyInteractedWith == false;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (showRange)
        {
            Vector3 wireOrigin = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            Handles.DrawWireDisc(wireOrigin, Vector3.forward, interactionSnapDistance);
        }
        if (Application.isPlaying)
        {
            Handles.Label(GetCurrentSnapPoint(), "X");
        }
#endif
    }

    public Vector2 GetCurrentSnapPoint()
    {
        Vector3 dogPos = _gameState.player.transform.position;
        Vector3 newPos = transform.position + (dogPos - transform.position).normalized * interactionSnapDistance;
        return newPos;
    }
}