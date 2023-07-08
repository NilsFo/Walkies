using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class OwnerPathWaypoint : MonoBehaviour
{
    public int index;
    private OwnerAI _ownerAI;
    private GameState _gameState;

    public bool currentWayPoint;
    private bool _lastKnownWayPointState;
    public UnityEvent onReached;

    public SpriteRenderer myRenderer;
    public Color onColor;
    public Color offColor;

    // Start is called before the first frame update
    void Start()
    {
        if (onReached == null)
        {
            onReached = new UnityEvent();
        }

        _gameState = FindObjectOfType<GameState>();
        _lastKnownWayPointState = false;
        currentWayPoint = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWayPoint != _lastKnownWayPointState)
        {
            OnWayPointActiveChange();
            _lastKnownWayPointState = currentWayPoint;
        }

        if (IsReached())
        {
            _gameState.OnWaypointReached(index);
            onReached.Invoke();
        }

        myRenderer.color = offColor;
        if (currentWayPoint)
        {
            myRenderer.color = onColor;
        }
    }

    public void OnWayPointActiveChange()
    {
        // print("Waypoint-Index " + index + " is now active: " + currentWayPoint);
    }

    public bool IsReached()
    {
        return Vector2.Distance(transform.position, _gameState.ownerAI.transform.position) <=
               _gameState.waypointReachedDistance
               && currentWayPoint;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string text = "WP-" + index;
        if (currentWayPoint)
        {
            text = text + "!";
        }

        Handles.Label(transform.position, text);
    }
#endif
}