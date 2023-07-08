using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBone : MonoBehaviour
{
    public GameObject ParticlePoofPrefab;
    private GameState _gamestate;
    public AudioClip collectedClip;
    public Color particleColor;

    // Start is called before the first frame update
    void Start()
    {
        _gamestate = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Collect()
    {
        _gamestate.OnBoneCollected();
        _gamestate.musicManager.CreateAudioClip(collectedClip, transform.position);
        
        GameObject ob = Instantiate(ParticlePoofPrefab, transform.position,Quaternion.identity);
        ob.GetComponent<ParticleSystem>().startColor = particleColor;
        
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        PlayerMovementBehaviour player = col.GetComponentInParent<PlayerMovementBehaviour>();
        if (player != null)
        {
            Collect();
        }
    }
}