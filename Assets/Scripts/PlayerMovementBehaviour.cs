using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementBehaviour : MonoBehaviour
{
    public enum PlayerInputState
    {
        Disabled,
        InControl,
        Frenzy,
        Free,
        AnimationLocked
    }

    public PlayerInputState currentInputState;
    private PlayerInputState _lastKnownInputState;

    [SerializeField] private float speed = 10;

    [SerializeField] private float dashMod = 5;
    [SerializeField] private float dashDuration = 2;
    private float _dashTime;

    public float movementCooldown = 0;

    private Vector2 _velocity;
    private Vector2 _lookDirection = new Vector2(1, 0);
    public Vector2 LookDirection => _lookDirection;
    public Vector2 velocity => _velocity;

    public float lineLength = 5f;
    public float linePullForce = 5f;

    private GameState _gameState;
    private OwnerAI _owner;
    private GamepadInputDetector _gamepadInputDetector;
    public AnimationCurve kbmMovementSmoothingCurve;

    public Rigidbody2D rb2D;

    public SpriteRenderer dogVisuals;
    public Animator dogAnimator;
    private static readonly int VelocityAnim = Animator.StringToHash("velocity");
    private static readonly int Sniff = Animator.StringToHash("sniff");

    public AudioClip ropeSnap;

    private void Awake()
    {
        currentInputState = PlayerInputState.InControl;
        _lastKnownInputState = currentInputState;
        _gameState = FindObjectOfType<GameState>();
        _gamepadInputDetector = FindObjectOfType<GamepadInputDetector>();
        _owner = _gameState.ownerAI;
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

        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (currentInputState == PlayerInputState.AnimationLocked)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        dogAnimator.SetFloat(VelocityAnim, _velocity.magnitude);

        // Frenzy Mode?
        if (FrenzyModeInputRequested() && currentInputState != PlayerInputState.AnimationLocked)
        {
            if (_gameState.FrenzyAvailable())
            {
                _gameState.EnterFrenzyMode();
                _gameState.playerSnoot.currentInteractable = null;
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

    private bool FrenzyModeInputRequested()
    {
        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;
            return gamepad.buttonWest.wasPressedThisFrame ||
                   gamepad.buttonEast.wasPressedThisFrame ||
                   gamepad.rightTrigger.wasPressedThisFrame ||
                   gamepad.leftTrigger.wasPressedThisFrame ||
                   gamepad.leftShoulder.wasPressedThisFrame ||
                   gamepad.rightShoulder.wasPressedThisFrame;
        }
        else
        {
            return Input.GetKeyDown(KeyCode.Space) ||
                   Input.GetKeyDown(KeyCode.Return);
        }
    }

    private void FixedUpdate()
    {
        var modSpeed = speed;
        if (_gameState.IsInFrenzyMode() || currentInputState == PlayerInputState.Free)
        {
            modSpeed *= _gameState.frenzySpeedMult;
        }

        if (_dashTime > 0)
        {
            modSpeed *= dashMod;
        }

        bool movementBlocked =
            !(currentInputState is PlayerInputState.InControl or PlayerInputState.Frenzy or PlayerInputState.Free);

        float x = 0;
        float y = 0;
        // if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        // {
        //     y += 1;
        //     if (_gameState.IsInFrenzyMode())
        //     {
        //         _gameState.playerMovedDuringFrenzy = true;
        //     }
        // }

        // if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        // {
        //     y -= 1;
        //     if (_gameState.IsInFrenzyMode())
        //     {
        //         _gameState.playerMovedDuringFrenzy = true;
        //     }
        // }

        // if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        // {
        //     x -= 1;
        //     if (_gameState.IsInFrenzyMode())
        //     {
        //         _gameState.playerMovedDuringFrenzy = true;
        //     }
        // }

        // if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        // {
        //     x += 1;
        //     if (_gameState.IsInFrenzyMode())
        //     {
        //         _gameState.playerMovedDuringFrenzy = true;
        //     }
        // }

        var inputHorizontalRaw = Input.GetAxisRaw("Horizontal");
        var inputVerticalRaw = Input.GetAxisRaw("Vertical");
        x = inputHorizontalRaw;
        y = inputVerticalRaw;

        if (_gameState.IsInFrenzyMode() && (inputHorizontalRaw != 0 || inputVerticalRaw == 0))
        {
            _gameState.playerMovedDuringFrenzy = true;
        }

        var moveInput = new Vector2(x, y).normalized * modSpeed;
        if (movementBlocked)
        {
            moveInput = Vector2.zero;
        }

        rb2D.AddForce(moveInput);
        _velocity = rb2D.velocity;
        //movementAnimator.velocity = _velocity;
        if (_velocity.magnitude > 0.01f && !movementBlocked)
            dogVisuals.transform.rotation =
                Quaternion.LookRotation(Vector3.forward, _velocity.normalized + 0.2f * moveInput);

        if (currentInputState != PlayerInputState.Free)
        {
            var ownerDelta = _owner.transform.position - transform.position;
            if (LeineStramm() && currentInputState != PlayerInputState.Frenzy)
            {
                // Leine einholen
                var pullForce = Mathf.Pow(ownerDelta.magnitude - lineLength + 1, 2) * linePullForce;
                rb2D.AddForce(ownerDelta.normalized * (pullForce * Time.deltaTime));
                _owner.DogPull();
            }
            else if (_owner.dogIsPulling)
            {
                _owner.DogStopPull();
            }

            if ((_owner.transform.position - transform.position).magnitude > lineLength * 3)
            {
                // Emergency reset
                if (currentInputState == PlayerInputState.InControl)
                {
                    transform.position = _owner.transform.position;
                    rb2D.velocity = Vector2.zero;
                }
                else
                {
                    _owner.transform.position = transform.position;
                }
            }
        }
    }

    public bool LeineStramm()
    {
        var ownerDelta = _owner.transform.position - transform.position;
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
        _gameState.playerSnoot.currentInteractable = null;
        dogAnimator.ResetTrigger(Sniff);
    }

    public void BreakFree()
    {
        // I want to break free

        currentInputState = PlayerInputState.Free;
        _owner.currentWalkingState = OwnerAI.WalkerMovementState.WaitingForDog;
        _gameState.OnEnterFrenzyMode(false, false);
        GetComponentInChildren<LineConnectionSprite>().enabled = false;
        _gameState.musicManager.Play(1);
        _gameState.playerSnoot.currentInteractable = null;

        _gameState.invisibleWallsHolder.SetActive(false);
        _gameState.winTriggerHolder.SetActive(true);

        _gameState.musicManager.CreateAudioClip(ropeSnap, transform.position);
    }
}