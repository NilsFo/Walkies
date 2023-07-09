using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public GameState gameState;
    public AudioClip audioClip;
    public GameObject leafsPoof;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameState.musicManager.CreateAudioClip(audioClip, 0.4f, true);
            Instantiate(leafsPoof, transform.position, Quaternion.identity);
        }
    }
}