using System.Collections;
using System.Collections.Generic;
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

    [FormerlySerializedAs("frenzyThreshold")] [Header("FrenzyCounter")]
    public int frenzyTokenThreshold = 2;

    [FormerlySerializedAs("frenzyProgress")]
    public int frenzyTokens = 0;

    public float frenzyTime = 5;
    public float frenzyTimeCurrent = 0;
    public bool playerMovedDuringFrenzy = false;

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
        }
        else
        {
            frenzyTimeCurrent = 0;
            playerMovedDuringFrenzy = false;
        }

        // Updating debug text
        debugTF.text = "Frenzy tokens: " + frenzyTokens + "/" + frenzyTokenThreshold + "\n" +
                       "Frenzy time: " + frenzyTimeCurrent + "\n" +
                       "Frenzy available: " + FrenzyAvailable()
            ;
    }

    public void EnterFrenzyMode()
    {
        frenzyTimeCurrent = frenzyTime;
        frenzyTokens = 0;
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

    public void OnWaypointReached(int index)
    {
        ownerTargetWaypointIndex = index + 1;
        if (ownerTargetWaypointIndex >= ownerPath.waypoints.Count)
        {
            print("WALKIES OVER!");
        }
    }
}