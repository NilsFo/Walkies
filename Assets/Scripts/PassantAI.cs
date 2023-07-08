using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassantAI : MonoBehaviour
{
    public enum WalkerMovementState
    {
        Unknown,
        WalkingHere,
        PettingDog
    }

    public Vector3 initialPos;

    public WalkerMovementState currentWalkingState = WalkerMovementState.Unknown;
    private WalkerMovementState _lastKnownWalkingState;

    public Transform walkTarget;

    private GameState _gameState;
    public float speed = 1f;
    public float turnSpeed = 1f;
    public float damp = 0.1f;
    private Vector2 _velocity;

    private PlayerMovementBehaviour dog;

    private Rigidbody2D rb2D;

    public Animator ownerAnimator;
    private static readonly int VelocityAnim = Animator.StringToHash("velocity");
    private static readonly int Pet = Animator.StringToHash("pet");

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
        dog = _gameState.player;
        initialPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWalkingState = WalkerMovementState.WalkingHere;
        _lastKnownWalkingState = currentWalkingState;
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        // Update States
        if (_lastKnownWalkingState != currentWalkingState)
        {
            _lastKnownWalkingState = currentWalkingState;
            print("New walker state: " + currentWalkingState);
        }

        switch (currentWalkingState)
        {
            case WalkerMovementState.WalkingHere:
                MovementWalkingHere();
                break;
            case WalkerMovementState.PettingDog:
                MovementLookAtDog();
                break;
        }
        
        ownerAnimator.SetFloat(VelocityAnim, _velocity.magnitude);
    }

    private void MovementLookAtDog()
    {
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        Vector2 myPos = transform.position;
        Vector2 targetDog = (Vector2)dog.transform.position - myPos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward,
            Vector3.RotateTowards((Vector2)transform.up,
                targetDog.normalized,
                turnSpeed * Time.deltaTime,
                0));

        _velocity -= _velocity * (damp * Time.deltaTime);
        rb2D.MovePosition(myPos + _velocity);
    }

    private void MovementWalkingHere()
    {
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        Vector2 myPos = transform.position;
        Vector2 target = walkTarget.position;
        Vector2 targetDirection = target - myPos;
        _velocity -= _velocity * (damp * Time.deltaTime);
        _velocity += (Vector2)transform.up * (speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(Vector3.forward,
            Vector3.RotateTowards((Vector2)transform.up,
                targetDirection.normalized,
                turnSpeed * Time.deltaTime,
                0));
        rb2D.MovePosition(myPos + _velocity);

        if ((target - myPos).magnitude < 1f)
        {
            Reset();
        }
    }

    public void PetDog()
    {
        Debug.Log("Petting da doge", this);
        currentWalkingState = WalkerMovementState.PettingDog;
        ownerAnimator.SetTrigger(Pet);
    }
    public void PetDogEnd()
    {
        currentWalkingState = WalkerMovementState.WalkingHere;
    }

    private void Reset()
    {
        transform.position = initialPos;
        GetComponent<Interactable>().alreadyInteractedWith = false;
    }
}