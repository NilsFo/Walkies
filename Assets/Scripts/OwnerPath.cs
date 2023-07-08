using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OwnerPath : MonoBehaviour
{
    public List<OwnerPathWaypoint> waypoints;
    private GameState _gameState;

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < waypoints.Count; i++)
        {
            OwnerPathWaypoint waypoint = waypoints[i];
            waypoint.index = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < waypoints.Count; i++)
        {
            OwnerPathWaypoint waypoint = waypoints[i];
            waypoint.currentWayPoint = false;
            if (i == _gameState.ownerTargetWaypointIndex)
            {
                waypoint.currentWayPoint = true;
            }
        }
    }

    public Vector3 CurrentWayPointTarget()
    {
        return waypoints[_gameState.ownerTargetWaypointIndex].transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (var i = 0; i < waypoints.Count - 1; i++)
        {
            OwnerPathWaypoint wp = waypoints[i];
            OwnerPathWaypoint nextWP = waypoints[i + 1];

            Debug.DrawLine(wp.transform.position, nextWP.transform.position);
        }
    }
#endif
}