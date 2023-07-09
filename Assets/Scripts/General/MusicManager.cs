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
    public readonly float GLOBAL_MUSIC_VOLUME_MULT = 0.6f;
    public readonly float GLOBAL_SOUND_VOLUME_MULT = 1.4f;

    [Range(0, 1)] public float levelVolumeMult = 1.0f;

    private AudioListener _listener;
    public List<AudioSource> initiallyKnownSongs;

    private List<AudioSource> _playList;
    public List<int> _desiredMixingVolumes;
    public float musicChangeSpeed = 1;

    private Dictionary<string, float> audioJail;

    private void Awake()
    {
        _playList = new List<AudioSource>();
        foreach (AudioSource song in initiallyKnownSongs)
        {
            _playList.Add(song);
            song.Play();
            song.volume = 0;
            _desiredMixingVolumes.Add(0);
        }

        SkipFade();

        _listener = FindObjectOfType<AudioListener>();
        audioJail = new Dictionary<string, float>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Play(int index, bool fromBeginning = false)
    {
        for (var i = 0; i < _playList.Count; i++)
        {
            _desiredMixingVolumes[i] = 0;
        }

        if (fromBeginning)
        {
            _playList[index].time = 0;
        }

        _desiredMixingVolumes[index] = 1;

        if (!_playList[index].isPlaying)
        {
            _playList[index].Play();
        }

        print("Playing: " + _playList[index].gameObject.name);
    }

    public void SkipFade()
    {
        for (var i = 0; i < _playList.Count; i++)
        {
            _playList[i].volume = _desiredMixingVolumes[i] * GetVolumeMusic() * levelVolumeMult;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (audioJail == null) return;
        transform.position = _listener.transform.position;
        userDesiredSoundVolume = MathF.Min(userDesiredMusicVolume * 1.0f, 1.0f);

        for (var i = 0; i < _playList.Count; i++)
        {
            var audioSource = _playList[i];
            var volumeMixing = _desiredMixingVolumes[i];

            var trueVolume = Mathf.Lerp(audioSource.volume,
                volumeMixing * GetVolumeMusic() * levelVolumeMult,
                Time.deltaTime * musicChangeSpeed);

            if (trueVolume - Time.deltaTime * musicChangeSpeed <= 0 && volumeMixing == 0)
            {
                trueVolume = 0;
            }

            audioSource.volume = trueVolume;
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

        string pg = "";
        foreach (var audioSource in _playList)
        {
            pg += " - " + audioSource.time;
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