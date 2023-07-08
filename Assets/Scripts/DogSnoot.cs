using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSnoot : MonoBehaviour
{
    public Interactable currentInteractable;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInteractable != null)
        {
            if (currentInteractable.IsInteractable())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Interact();
                }
            }
            else
            {
                currentInteractable = null;
            }
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