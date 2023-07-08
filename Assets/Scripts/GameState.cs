using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [Header("Walking Path")] public OwnerAI ownerAI;
    public OwnerPath ownerPath;
    public int ownerTargetWaypointIndex;
    public float waypointReachedDistance = 0.42f;

    [Header("Player")] public PlayerMovementBehaviour player;
    public DogSnoot playerSnoot;

    // Start is called before the first frame update
    void Start()
    {
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