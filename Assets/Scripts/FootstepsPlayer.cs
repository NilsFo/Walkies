using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FootstepsPlayer : MonoBehaviour
{
    public string targetName = "werewolf_rig|werewolf_walk";
    public List<AudioClip> footstepClips;
    public int currentAnimFrame;

    private float m_CurrentClipLength;
    private bool playSoundNextFrame;

    public Animator m_Animator;
    private string m_ClipName;
    AnimatorClipInfo[] m_CurrentClipInfo;

    public int leftFootProgress;
    public int rightFootProgress;
    private GameState _gameState;

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playSoundNextFrame = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Fetch the current Animation clip information for the base layer
        m_CurrentClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
        //Access the current length of the clip
        m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
        //Access the Animation clip name
        m_ClipName = m_CurrentClipInfo[0].clip.name;

        if (m_ClipName == targetName)
        {
            var animProgress = m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            // animProgress = (float)System.Math.Round(animProgress,1);
            animProgress = animProgress % 1;
            currentAnimFrame = (int)(animProgress * 24);
            //print("foot frame: " + currentAnimFrame);
            if (currentAnimFrame == leftFootProgress || currentAnimFrame == rightFootProgress)
            {
                int i = Random.Range(0, footstepClips.Count);
                AudioClip step = footstepClips[i];

                _gameState.musicManager.CreateAudioClip(step, transform.position);
            }
            else
            {
                currentAnimFrame = 0;
            }
        }
        else
        {
            currentAnimFrame = 0;
        }
    }
}