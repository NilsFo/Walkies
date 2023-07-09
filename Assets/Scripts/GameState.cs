using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameState : MonoBehaviour
{
    [Header("Gameplay")] public int targetFPS = 60;

    [Header("Walking Path")] public OwnerAI ownerAI;
    public OwnerPath ownerPath;
    public int ownerTargetWaypointIndex;
    public float waypointReachedDistance = 0.42f;

    [Header("Player")] public PlayerMovementBehaviour player;
    public DogSnoot playerSnoot;
    public MusicManager musicManager;
    public TMP_Text debugTF;

    [Header("Interactions")] public Dictionary<Interactable.InteractableType, int> InteractionsCount;

    [Header("FrenzyCounter")] public int frenzyTokenThreshold = 2;
    public int frenzyTokens = 0;
    public float frenzySpeedMult = 2.0f;

    public float frenzyTime = 5;
    public float frenzyTimeCurrent = 0;
    public bool playerMovedDuringFrenzy = false;

    [Header("Camera")] public CinemachineVirtualCamera virtualCamera;
    public float cameraDistanceDefault = 5f;
    public float cameraDistanceFrenzy = 7f;
    public float zoomSpeed = 2f;

    [Header("Bones")] public int bonesCollectedCount = 0;
    public int bonesCollectedTarget = 0;

    [Header("Winning")] public GameObject invisibleWallsHolder;
    public GameObject winTriggerHolder;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFPS;

        InteractionsCount = new Dictionary<Interactable.InteractableType, int>
        {
            { Interactable.InteractableType.Friend, 0 },
            { Interactable.InteractableType.Pet, 0 },
            { Interactable.InteractableType.Treat, 0 },
            { Interactable.InteractableType.Tree, 0 }
        };

        ownerTargetWaypointIndex = -1;
        StartWalkies();

        musicManager.Play(0);
        musicManager.SkipFade();

        // Updating bones objective
        var bones = FindObjectsOfType<CollectibleBone>();
        bonesCollectedTarget = bones.Length;
    }

    public void StartWalkies()
    {
        ownerTargetWaypointIndex = 0;
    }

    public void FinishWalkies()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInFrenzyMode())
        {
            frenzyTimeCurrent -= Time.deltaTime;
            if (frenzyTimeCurrent <= 0)
            {
                OnExitFrenzyMode();
            }
        }
        else
        {
            frenzyTimeCurrent = 0;
            playerMovedDuringFrenzy = false;
        }

        // Updating debug text
        debugTF.text = bonesCollectedCount + "/" + bonesCollectedTarget;
    }

    private void LateUpdate()
    {
        float currentOrthographicDistance = virtualCamera.m_Lens.OrthographicSize;
        float desiredOrthographicDistance = cameraDistanceDefault;
        if (IsInFrenzyMode())
        {
            desiredOrthographicDistance = cameraDistanceFrenzy;
        }

        currentOrthographicDistance = Mathf.MoveTowards(currentOrthographicDistance, desiredOrthographicDistance,
            Time.deltaTime * zoomSpeed);
        virtualCamera.m_Lens.OrthographicSize = currentOrthographicDistance;
    }

    public void EnterFrenzyMode()
    {
        frenzyTimeCurrent = frenzyTime;
        frenzyTokens = 0;
        OnEnterFrenzyMode();
    }

    public void OnEnterFrenzyMode()
    {
        musicManager.Play(1, true);
        musicManager.SkipFade();
    }

    public void OnExitFrenzyMode()
    {
        print("OnFrenzyExit!");
        musicManager.Play(0);
        
        // Set closest waypoint
        int closestIndex = ownerPath.waypoints.IndexOf(ownerPath.waypoints.OrderBy(waypoint =>
            Vector2.Distance(waypoint.transform.position, ownerAI.transform.position)).FirstOrDefault());
        ownerTargetWaypointIndex = closestIndex;
    }

    public bool IsInFrenzyMode()
    {
        return frenzyTimeCurrent > 0;
    }

    public bool FrenzyAvailable()
    {
        return frenzyTokens >= frenzyTokenThreshold;
    }

    public void AddFrenzyPoint()
    {
        frenzyTokens += 1;
    }

    public float GetFrenzyTimeRemainingPercent()
    {
        return frenzyTimeCurrent / frenzyTime;
    }

    public float GetFrenzyTokenProgressPercent()
    {
        return (float)frenzyTokens / (float)frenzyTokenThreshold;
    }

    public void OnBoneCollected()
    {
        bonesCollectedCount++;
        if (bonesCollectedCount >= bonesCollectedTarget)
        {
            player.BreakFree();
        }
    }

    public void OnWaypointReached(int index)
    {
        ownerTargetWaypointIndex = index + 1;
        if (ownerTargetWaypointIndex >= ownerPath.waypoints.Count)
        {
            print("WALKIES OVER! Restarting!");
            ownerTargetWaypointIndex = 0;
        }
        ownerAI.fastWalk = false;
    }

    public void Win()
    {
        Debug.Log("u win");
    }
}
