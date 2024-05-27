using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool alreadyInteractedWith;

    public InteractableType myType;
    public AnimationName animationPlaying;
    private GameState _gameState;
    public UnityEvent onInteractedWith;
    public UnityEvent onInteractionEnd;

    [Header("Interactions")] public float interactionSnapDistance = 1.0f;
    public bool showRange = false;
    public bool barkAfterwards = false;
    public bool meowAfterwards = false;
    public AudioClip meow;

    [Header("Cooldown")] public float reuseCooldown = 25;
    private float reuseCooldownCurrent = 0;

    [Header("UI")] public GameObject keepUprightUI;

    [Header("Debug")] public GameObject focusHighlightVisuals;

    private static readonly int VelocityAnim = Animator.StringToHash("velocity");
    private static readonly int Sniff = Animator.StringToHash("sniff");
    private static readonly int Sit = Animator.StringToHash("sit");
    private static readonly int Pee = Animator.StringToHash("pee");

    public enum InteractableType
    {
        Unknown,
        Pet,
        Treat,
        Tree,
        Friend
    }

    public enum AnimationName
    {
        Unknown,
        Sit,
        Pee,
        Sniff
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameState>();
        alreadyInteractedWith = false;
        if (onInteractedWith == null)
        {
            onInteractedWith = new UnityEvent();
        }

        if (myType == InteractableType.Unknown)
        {
            Debug.LogError("THIS GAME OBJECT IS DEFAULT INTERACTABLE", gameObject);
        }

        if (animationPlaying == AnimationName.Unknown)
        {
            Debug.LogError("THIS GAME OBJECT IS DEFAULT ANIMATION", gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        focusHighlightVisuals.SetActive(false);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, -8f);
        keepUprightUI.transform.position = pos;
        Quaternion q = Quaternion.identity;
        keepUprightUI.transform.rotation = q;

        if (alreadyInteractedWith)
        {
            reuseCooldownCurrent -= Time.deltaTime;
            if (reuseCooldownCurrent < 0)
            {
                alreadyInteractedWith = false;
            }
        }
        else
        {
            reuseCooldownCurrent = reuseCooldown;
        }
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

        float lockTime = AnimationNameToLength(animationPlaying);

        // Play animation
        Vector2 lookTarget = transform.position;
        if (animationPlaying == AnimationName.Pee)
        {
            Vector2 playerPos = _gameState.player.transform.position;
            lookTarget = -Vector2.Perpendicular(((Vector2)playerPos - lookTarget)) + playerPos;
        }

        _gameState.player.PlayLockedAnimation(GetCurrentSnapPoint(),
            lookTarget,
            AnimationNameToID(animationPlaying),
            lockTime);

        Invoke(nameof(OnInteractionEnd), lockTime);
        // Add frenzy point
        _gameState.AddFrenzyPoint();
    }

    public void OnInteractionEnd()
    {
        onInteractionEnd.Invoke();
        if (barkAfterwards)
        {
            RequestBark();
        }

        if (meowAfterwards)
        {
            _gameState.musicManager.CreateAudioClip(meow, transform.position);
        }
    }

    public bool IsInteractable()
    {
        return alreadyInteractedWith == false && !_gameState.IsInFrenzyMode() && !_gameState.IsInFreeMode();
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

    public float AnimationNameToLength(AnimationName animationName)
    {
        switch (animationName)
        {
            case AnimationName.Pee:
                return 2.3f;
            case AnimationName.Sniff:
                return 1.5f;
            case AnimationName.Sit:
                return 2.5f;
        }

        Debug.LogError("NO ANIMATION LENGTH SET");
        return 0f;
    }

    public int AnimationNameToID(AnimationName animantionName)
    {
        switch (animantionName)
        {
            case AnimationName.Pee:
                return Pee;
            case AnimationName.Sniff:
                return Sniff;
            case AnimationName.Sit:
                return Sit;
        }

        Debug.LogError("NO ANIMATION ID SET");
        return 0;
    }

    public Vector2 GetCurrentSnapPoint()
    {
        Vector2 dogPos = _gameState.player.transform.position;
        Vector2 newPos = (Vector2)transform.position +
                         (dogPos - (Vector2)transform.position).normalized * interactionSnapDistance;
        return newPos;
    }

    public void RequestBark()
    {
        _gameState.playerSnoot.RequestBark();
    }
}