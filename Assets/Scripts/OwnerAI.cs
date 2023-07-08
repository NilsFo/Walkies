using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnerAI : MonoBehaviour
{
    public enum WalkerMovementState
    {
        Unknown,
        WalkingHere,
        WaitingForDog,
        Flailing
    }

    public WalkerMovementState currentWalkingState = WalkerMovementState.Unknown;
    private WalkerMovementState _lastKnownWalkingState;

    private GameState _gameState;
    public float speed = 1f;
    public float turnSpeed = 1f;
    public float damp = 0.1f;
    private Vector2 _velocity;

    private PlayerMovementBehaviour dog;
    public bool dogIsPulling;
    public float dogPullSurpriseTime = 0.5f;
    public float dogPullCooldownTime = 5f;
    private float _dogPullSurpriseTimer = 0.0f;
    private float _dogPullCooldownTimer = 0.0f;

    private Rigidbody2D rb2D;

    public Animator ownerAnimator;
    private static readonly int VelocityAnim = Animator.StringToHash("velocity");

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
        dog = _gameState.player;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWalkingState = WalkerMovementState.WalkingHere;
        _lastKnownWalkingState = currentWalkingState;
        rb2D = GetComponent<Rigidbody2D>();
    }

    private bool IsDogAnimationLocked()
    {
        return dog.currentInputState == PlayerMovementBehaviour.PlayerInputState.AnimationLocked;
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
            case WalkerMovementState.WaitingForDog:
                MovementLookAtDog();
                break;
            case WalkerMovementState.Flailing:
                // TODO implement
                break;
        }
        
        ownerAnimator.SetFloat(VelocityAnim, _velocity.magnitude);
    }

    private void MovementLookAtDog()
    {
        Vector2 myPos = transform.position;
        Vector2 targetDog = (Vector2)dog.transform.position - myPos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward,
            Vector3.RotateTowards((Vector2)transform.up,
                targetDog.normalized,
                turnSpeed * Time.deltaTime,
                0));

        // _velocity *= 1-damp*Time.deltaTime;
        // rb2D.MovePosition(myPos + _velocity);
    }

    private void MovementWalkingHere()
    {
        Vector2 myPos = transform.position;
        Vector2 target = _gameState.ownerPath.CurrentWayPointTarget();
        Vector2 targetDirection = target - myPos;
        _velocity -= _velocity * (damp * Time.deltaTime);
        _velocity += (Vector2)transform.up * (speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(Vector3.forward,
            Vector3.RotateTowards((Vector2)transform.up,
                targetDirection.normalized,
                turnSpeed * Time.deltaTime,
                0));
        rb2D.MovePosition(myPos + _velocity);

        if (dogIsPulling && _dogPullSurpriseTimer > 0)
        {
            _dogPullSurpriseTimer -= Time.deltaTime;
            Vector2 targetDog = (Vector2)dog.transform.position - myPos;
            transform.rotation = Quaternion.LookRotation(Vector3.forward,
                Vector3.RotateTowards((Vector2)transform.up,
                    targetDog.normalized,
                    turnSpeed * Time.deltaTime * 5f,
                    0));

            if (IsDogAnimationLocked())
            {
                currentWalkingState = WalkerMovementState.WaitingForDog;
            }
        }

        if (_dogPullCooldownTimer > 0)
        {
            _dogPullCooldownTimer -= Time.deltaTime;
        }
    }

    public void DogPull()
    {
        if (!dogIsPulling && _dogPullCooldownTimer <= 0)
        {
            dogIsPulling = true;
            _dogPullSurpriseTimer = dogPullSurpriseTime;
            _dogPullCooldownTimer = dogPullCooldownTime;
        }
    }

    public void DogStopPull()
    {
        dogIsPulling = false;
    }
}