using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        barkCooldownCurrent = barkCooldownCurrent - Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
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

    public void RequestBark()
    {
        if (barkCooldownCurrent <= 0)
        {
            AudioClip bark = barks[0];
            _gameState.musicManager.CreateAudioClip(bark, transform.position);
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
            if (interactable.IsInteractable())
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