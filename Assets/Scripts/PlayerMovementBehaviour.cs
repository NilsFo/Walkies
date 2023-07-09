using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{
    public enum PlayerInputState
    {
        Disabled,
        InControl,
        Frenzy,
        AnimationLocked
    }

    public PlayerInputState currentInputState;
    private PlayerInputState _lastKnownInputState;

    [SerializeField] private float speed = 10;

    [SerializeField] private float dashMod = 5;
    [SerializeField] private float dashDuration = 2;
    private float _dashTime;

    public float movementCooldown = 0;

    Vector2 _velocity;

    Vector2 _lookDirection = new Vector2(1, 0);
    public Vector2 lookDirection => _lookDirection;
    public Vector2 velocity => _velocity;


    public float lineLength = 5f;
    public float linePullForce = 5f;

    private GameState _gameState;
    private OwnerAI owner;

    public Rigidbody2D rb2D;

    public SpriteRenderer dogVisuals;
    public Animator dogAnimator;
    private static readonly int VelocityAnim = Animator.StringToHash("velocity");
    private static readonly int Sniff = Animator.StringToHash("sniff");


    private void Awake()
    {
        currentInputState = PlayerInputState.InControl;
        _lastKnownInputState = currentInputState;
        _gameState = FindObjectOfType<GameState>();
        owner = _gameState.ownerAI;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (_lastKnownInputState != currentInputState)
        {
            _lastKnownInputState = currentInputState;
            print("New Dog input state: " + currentInputState);
        }

        rb2D.constraints = RigidbodyConstraints2D.None;
        if (currentInputState == PlayerInputState.AnimationLocked)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        dogAnimator.SetFloat(VelocityAnim, _velocity.magnitude);

        // Frenzy Mode?
        if (Input.GetKeyDown(KeyCode.Space) && currentInputState != PlayerInputState.AnimationLocked)
        {
            if (_gameState.FrenzyAvailable())
            {
                _gameState.EnterFrenzyMode();
            }
            else
            {
                _gameState.playerSnoot.RequestBark();
            }
        }

        if (_gameState.IsInFrenzyMode() && currentInputState == PlayerInputState.InControl)
        {
            currentInputState = PlayerInputState.Frenzy;
        }

        if (!_gameState.IsInFrenzyMode() && currentInputState == PlayerInputState.Frenzy)
        {
            currentInputState = PlayerInputState.InControl;
        }
    }

    private void FixedUpdate()
    {
        var modSpeed = speed;
        if (_gameState.IsInFrenzyMode())
        {
            modSpeed *= _gameState.frenzySpeedMult;
        }
        
        if (_dashTime > 0)
        {
            modSpeed *= dashMod;
        }

        bool movementBlocked = true;
        if (currentInputState == PlayerInputState.InControl)
        {
            movementBlocked = false;
        }

        if (currentInputState == PlayerInputState.Frenzy)
        {
            movementBlocked = false;
        }

        float x = 0;
        float y = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            y += 1;
            if (_gameState.IsInFrenzyMode())
            {
                _gameState.playerMovedDuringFrenzy = true;
            }
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            y -= 1;
            if (_gameState.IsInFrenzyMode())
            {
                _gameState.playerMovedDuringFrenzy = true;
            }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            x -= 1;
            if (_gameState.IsInFrenzyMode())
            {
                _gameState.playerMovedDuringFrenzy = true;
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            x += 1;
            if (_gameState.IsInFrenzyMode())
            {
                _gameState.playerMovedDuringFrenzy = true;
            }
        }

        var moveInput = new Vector2(x, y).normalized * modSpeed;
        if (movementBlocked)
        {
            moveInput = Vector2.zero;
        }

        rb2D.AddForce(moveInput);
        _velocity = rb2D.velocity;
        //movementAnimator.velocity = _velocity;
        if (_velocity.sqrMagnitude != 0)
            dogVisuals.transform.rotation =
                Quaternion.LookRotation(Vector3.forward, _velocity.normalized + 0.2f * moveInput);

        var ownerDelta = owner.transform.position - transform.position;
        if (LeineStramm() && currentInputState != PlayerInputState.Frenzy)
        {
            // Leine einholen
            var pullForce = Mathf.Pow(ownerDelta.magnitude - lineLength + 1, 2) * linePullForce;
            rb2D.AddForce(ownerDelta.normalized * (pullForce * Time.deltaTime));
            owner.DogPull();
        }
        else if (owner.dogIsPulling)
        {
            owner.DogStopPull();
        }

        if ((owner.transform.position - transform.position).magnitude > lineLength * 3)
        {
            // Emergency reset
            if (currentInputState == PlayerInputState.InControl)
            {
                transform.position = owner.transform.position;
                rb2D.velocity = Vector2.zero;
            }
            else
            {
                owner.transform.position = transform.position;
            }
        }
    }

    public bool LeineStramm()
    {
        var ownerDelta = owner.transform.position - transform.position;
        return ownerDelta.magnitude > lineLength;
    }

    public void PlayLockedAnimation(int animationID, float lockTime)
    {
        PlayLockedAnimation(transform.position, transform.position, animationID, lockTime);
    }

    public void PlayLockedAnimation(Vector2 newPosition, Vector2 rotateTowards, int animationID, float lockTime)
    {
        // Set position
        Vector3 newPos = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        Vector3 targetPos = new Vector3(rotateTowards.x, rotateTowards.y, transform.position.z);
        transform.position = newPos;

        // Set Rotation
        dogVisuals.transform.rotation = Quaternion.LookRotation(Vector3.forward, (targetPos - newPos).normalized);

        // Set lock
        currentInputState = PlayerInputState.AnimationLocked;
        dogAnimator.SetTrigger(animationID);
        Invoke(nameof(ReleaseAnimationLock), lockTime);
    }

    public void ReleaseAnimationLock()
    {
        currentInputState = PlayerInputState.InControl;
        dogAnimator.ResetTrigger(Sniff);
    }
}