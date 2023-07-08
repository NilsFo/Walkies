using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public GameObject temporalAudioPlayerPrefab;
    public static float userDesiredMusicVolume = 0.5f;
    public static float userDesiredSoundVolume = 0.5f;
    public readonly float GLOBAL_MUSIC_VOLUME_MULT = 0.42f;
    public readonly float GLOBAL_SOUND_VOLUME_MULT = 1.69001337f;

    [Range(0, 1)] public float levelVolumeMult = 1.0f;

    private AudioListener _listener;
    public AudioSource song;
    private List<AudioSource> playList;
    private Dictionary<string, float> audioJail;

    private void Awake()
    {
        playList = new List<AudioSource>();
        if (song != null)
        {
            playList.Add(song);
        }

        _listener = FindObjectOfType<AudioListener>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioJail = new Dictionary<string, float>();
    }

    private void Play(int i)
    {
        foreach (AudioSource audioSource in playList)
        {
            audioSource.Stop();
        }

        playList[i].Play();
    }

    public void PlaySongLoop()
    {
        Play(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (audioJail == null) return;
        transform.position = _listener.transform.position;
        userDesiredSoundVolume = MathF.Min(userDesiredMusicVolume * 1.0f, 1.0f);

        foreach (var audioSource in playList)
        {
            audioSource.volume = GetVolumeMusic() * levelVolumeMult;
        }

        var keys = audioJail.Keys.ToArrayPooled().ToList();
        List<string> releaseKeys = new List<string>();
        if (keys.Count > 0)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                float timeout = audioJail[key];
                timeout -= Time.deltaTime;
                audioJail[key] = timeout;

                if (timeout < 0)
                {
                    releaseKeys.Add(key);
                }
            }
        }

        foreach (var releaseKey in releaseKeys)
        {
            audioJail.Remove(releaseKey);
        }
    }

    public float GetVolumeMusic()
    {
        return userDesiredMusicVolume * GLOBAL_MUSIC_VOLUME_MULT;
    }

    public float GetVolumeSound()
    {
        return userDesiredSoundVolume * GLOBAL_SOUND_VOLUME_MULT;
    }

    public void CreateAudioClip(AudioClip audioClip, float soundInstanceVolumeMult = 1.0f, bool respectBinning = false)
    {
        CreateAudioClip(audioClip, transform.position, soundInstanceVolumeMult, respectBinning);
    }

    public void CreateAudioClip(AudioClip audioClip, Vector3 position, float soundInstanceVolumeMult = 1.0f,
        bool respectBinning = false)
    {
        // Registering in the jail
        string clipName = audioClip.name;
        float jailTime = audioClip.length * 0.42f;
        if (audioJail.ContainsKey(clipName))
        {
            audioJail[clipName] = jailTime;
            if (respectBinning)
            {
                return;
            }
        }
        else
        {
            audioJail.Add(clipName, jailTime);
        }

        // Instancing the sound
        GameObject adp = Instantiate(temporalAudioPlayerPrefab);
        adp.transform.position = position;
        AudioSource source = adp.GetComponent<AudioSource>();
        TimedLife life = adp.GetComponent<TimedLife>();
        life.aliveTime = audioClip.length * 2;
        source.clip = audioClip;
        source.volume = MathF.Min(GetVolumeSound() * soundInstanceVolumeMult * levelVolumeMult, 1.0f);
        source.Play();
    }
}