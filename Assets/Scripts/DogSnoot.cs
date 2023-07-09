using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogSnoot : MonoBehaviour
{
    public Interactable currentInteractable;
    private GameState _gameState;

    public List<AudioClip> barks;
    public float barkCooldown = 2f;
    public float barkCooldownCurrent = 0f;
    private static readonly int Sniff = Animator.StringToHash("sniff");

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        barkCooldownCurrent = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float cooldownMult = 1.0f;
        if (_gameState.IsInFrenzyMode() || _gameState.IsInFreeMode())
        {
            cooldownMult = 2.5f;
        }

        barkCooldownCurrent = barkCooldownCurrent - Time.deltaTime * cooldownMult;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null && !_gameState.IsInFrenzyMode())
            {
                if (currentInteractable.IsInteractable())
                {
                    Interact();
                }
                else
                {
                    currentInteractable = null;
                }
            }
            else
            {
                RequestBark();
            }
        }
    }

    private void LateUpdate()
    {
        if (currentInteractable != null)
        {
            currentInteractable.focusHighlightVisuals.SetActive(currentInteractable.IsInteractable());
        }
    }

    public void RequestBark()
    {
        if (barkCooldownCurrent <= 0 && _gameState.HasGameStartedYet())
        {
            int i = Random.Range(0, barks.Count);
            AudioClip bark = barks[i];

            _gameState.musicManager.CreateAudioClip(bark, transform.position, soundInstanceVolumeMult: 1.8f);
            barkCooldownCurrent = barkCooldown;
        }
    }

    private void Interact()
    {
        currentInteractable.OnInteractedWith();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Interactable interactable = col.GetComponent<Interactable>();
        if (interactable == null)
        {
            interactable = col.GetComponentInParent<Interactable>();
        }

        if (interactable != null)
        {
            if (interactable.IsInteractable() && _gameState.player.currentInputState !=
                PlayerMovementBehaviour.PlayerInputState.AnimationLocked)
            {
                currentInteractable = interactable;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null)
        {
            interactable = other.GetComponentInParent<Interactable>();
        }

        if (interactable != null)
        {
            if (interactable.IsInteractable())
            {
                currentInteractable = null;
            }
        }
    }
}