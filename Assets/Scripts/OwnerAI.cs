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
    public bool fastWalk = false;
    public float turnSpeed = 1f;
    public float damp = 0.1f;
    public float flailPullForce = 10f;
    public float flailMaxVelocity = 100f;
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
    private static readonly int TumbleAnim = Animator.StringToHash("tumble");

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

    private bool IsDogFrenzy()
    {
        return dog.currentInputState == PlayerMovementBehaviour.PlayerInputState.Frenzy;
    }

    // Update is called once per frame
    void Update()
    {
        // Stop waiting, if released
        if (!IsDogAnimationLocked() && currentWalkingState == WalkerMovementState.WaitingForDog)
        {
            currentWalkingState = WalkerMovementState.WalkingHere;
        }

        PlayerMovementBehaviour player = _gameState.player;
        if (player.LeineStramm() && IsDogFrenzy() && _gameState.playerMovedDuringFrenzy && currentWalkingState == WalkerMovementState.WalkingHere)
        {
            currentWalkingState = WalkerMovementState.Flailing;
            rb2D.velocity = Vector2.zero;
        }

        if (currentWalkingState == WalkerMovementState.Flailing && !IsDogFrenzy())
        {
            currentWalkingState = WalkerMovementState.WalkingHere;
        }

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
                MovementFlailing();
                break;
        }
        
        ownerAnimator.SetFloat(VelocityAnim, _velocity.magnitude);
        ownerAnimator.SetBool(TumbleAnim, currentWalkingState == WalkerMovementState.Flailing);
    }

    private void MovementFlailing()
    {
        fastWalk = true;
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        Vector2 myPos = transform.position;
        _velocity -= _velocity * (damp * Time.deltaTime);
        // rb2D.MovePosition(myPos + _velocity);

        PlayerMovementBehaviour player = _gameState.player;
        var ownerDelta = player.transform.position - transform.position;
        if (player.LeineStramm())
        {
            var pullForce = Mathf.Pow(ownerDelta.magnitude - player.lineLength + 1, 2f) * flailPullForce;
            rb2D.AddForce(ownerDelta.normalized * (pullForce * Time.deltaTime));
            rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, flailMaxVelocity);
        }
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
        rb2D.MovePosition(myPos + _velocity * Time.deltaTime);
    }

    private void MovementWalkingHere()
    {
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        Vector2 myPos = transform.position;
        Vector2 target = _gameState.ownerPath.CurrentWayPointTarget();
        Vector2 targetDirection = target - myPos;
        _velocity -= _velocity * (damp * Time.deltaTime);
        _velocity += (Vector2)transform.up * (speed * (fastWalk ? 2f : 1f));
        transform.rotation = Quaternion.LookRotation(Vector3.forward,
            Vector3.RotateTowards((Vector2)transform.up,
                targetDirection.normalized,
                turnSpeed * Time.deltaTime,
                0));
        
        Vector2 targetDog = (Vector2)dog.transform.position - myPos;
        if (dogIsPulling && _dogPullSurpriseTimer > 0)
        {
            _dogPullSurpriseTimer -= Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(Vector3.forward,
                Vector3.RotateTowards((Vector2)transform.up,
                    targetDog.normalized,
                    turnSpeed * Time.deltaTime * 5f,
                    0));
        }

        if (dogIsPulling)
        {
            var pullForce = Mathf.Clamp(Mathf.Pow(targetDog.magnitude - dog.lineLength + 1, 2) * 0.5f, 0f, 0.3f);
            _velocity += targetDog.normalized * pullForce;
        }

        rb2D.MovePosition(myPos + _velocity * Time.deltaTime);
        
        if (_dogPullCooldownTimer > 0)
        {
            _dogPullCooldownTimer -= Time.deltaTime;
        }
        
        if (IsDogAnimationLocked())
        {
            currentWalkingState = WalkerMovementState.WaitingForDog;
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