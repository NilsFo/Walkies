using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLooper : MonoBehaviour
{
    public MusicManager musicManager;
    public float volume;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.loop = false;
        NextAudioSource();
    }

    // Update is called once per frame
    void Update()
    {
        float localVolume = musicManager.GetVolumeSound() * volume;
        audioSource.volume = localVolume;

        if (!audioSource.isPlaying)
        {
            NextAudioSource();
        }
    }

    public void NextAudioSource()
    {
        audioSource.Stop();

        int i = Random.Range(0, audioClips.Count);
        var clip = audioClips[i];
        audioSource.clip = clip;
        audioSource.Play();

        print("New ambient audio track: " + clip.name);
    }
}