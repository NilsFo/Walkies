using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [Header("Gameplay")] public int targetFPS = 60;

    [Header("Walking Path")] public OwnerAI ownerAI;
    public OwnerPath ownerPath;
    public int ownerTargetWaypointIndex;
    public float waypointReachedDistance = 0.42f;

    [Header("Player")] public PlayerMovementBehaviour player;
    public DogSnoot playerSnoot;

    [Header("Interactions")] public Dictionary<Interactable.InteractableType, int> InteractionsCount;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFPS;

        InteractionsCount = new Dictionary<Interactable.InteractableType, int>();
        InteractionsCount.Add(Interactable.InteractableType.Friend, 0);
        InteractionsCount.Add(Interactable.InteractableType.Pet, 0);
        InteractionsCount.Add(Interactable.InteractableType.Treat, 0);
        InteractionsCount.Add(Interactable.InteractableType.Tree, 0);

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