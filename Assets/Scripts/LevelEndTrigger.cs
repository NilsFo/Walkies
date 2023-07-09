using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private GameState _gameState;
    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var dog = other.GetComponentInParent<PlayerMovementBehaviour>();
        if (dog != null && dog.currentInputState == PlayerMovementBehaviour.PlayerInputState.Free)
        {
            // End this

            _gameState.Win();
        }
    }
}
