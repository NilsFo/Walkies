using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnerAI : MonoBehaviour
{
    private GameState _gameState;
    public float speed = 1f;
    public float turnSpeed = 1f;
    public float damp = 0.1f;
    private Vector2 _velocity;

    public PlayerMovementBehaviour dog;
    public bool dogIsPulling;
    public float dogPullSurpriseTime = 0.5f;
    public float dogPullCooldownTime = 5f;
    private float _dogPullSurpriseTimer = 0.0f;
    private float _dogPullCooldownTimer = 0.0f;

    private Rigidbody2D rb2D;
    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameState>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 myPos = transform.position;
        Vector2 target = _gameState.ownerPath.CurrentWayPointTarget();
        Vector2 targetDirection = target - myPos;
        _velocity *= damp;
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
