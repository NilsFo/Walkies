using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIGamepadSliderRelay : MonoBehaviour
{
    private GamepadInputDetector _gamepadInputDetector;

    [Header("Hookup")] public Slider mySlider;
    private EventSystem _eventSystem;

    [Header("Gamepad Button Pairs to listen for:")]
    public bool triggers = false;

    public bool shoulderButtons = false;
    public bool dPadHorizontal = false;
    public bool dPadVertical = false;
    private Vector2Int _dpadLastFrame;

    [Header("Config")] public float gamepadMagnitudeTap;
    public float gamepadMagnitudeHold;
    public float holdRegisterDelay = 0.25f;
    private float _holdRegisterDelay = 0f;
    public bool inverted;

    private void Awake()
    {
        _gamepadInputDetector = FindObjectOfType<GamepadInputDetector>();
    }

    private void Start()
    {
        _dpadLastFrame = Vector2Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        _holdRegisterDelay -= Time.deltaTime;

        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;

            if (gamepad != null)
            {
                Vector2 dpadRaw = gamepad.dpad.ReadValue();
                Vector2Int dpad = new Vector2Int((int)dpadRaw.x, (int)dpadRaw.y);

                // Checking if controlled by triggers
                if (triggers)
                {
                    if (gamepad.leftTrigger.wasPressedThisFrame)
                    {
                        Tap(true);
                    }
                    else if (gamepad.leftTrigger.isPressed)
                    {
                        Hold(true);
                    }

                    if (gamepad.rightTrigger.wasPressedThisFrame)
                    {
                        Tap(false);
                    }
                    else if (gamepad.rightTrigger.isPressed)
                    {
                        Hold(false);
                    }
                }

                // Checking if controlled by shoulder buttons
                if (shoulderButtons)
                {
                    if (gamepad.leftShoulder.wasPressedThisFrame)
                    {
                        Tap(true);
                    }
                    else if (gamepad.leftShoulder.isPressed)
                    {
                        Hold(true);
                    }

                    if (gamepad.rightShoulder.wasPressedThisFrame)
                    {
                        Tap(false);
                    }
                    else if (gamepad.rightShoulder.isPressed)
                    {
                        Hold(false);
                    }
                }

                // Checking if controlled by dpad horizontal
                if (dPadHorizontal)
                {
                    if (DpadLeftWasPressedThisFrame(dpad))
                    {
                        Tap(true);
                    }
                    else if (DpadLeftIsPressed(dpad))
                    {
                        Hold(true);
                    }

                    if (DpadRightWasPressedThisFrame(dpad))
                    {
                        Tap(false);
                    }
                    else if (DpadRightIsPressed(dpad))
                    {
                        Hold(false);
                    }
                }
                
                // Checking if controlled by dpad horizontal
                if (dPadVertical)
                {
                    if (DpadDownWasPressedThisFrame(dpad))
                    {
                        Tap(true);
                    }
                    else if (DpadDownIsPressed(dpad))
                    {
                        Hold(true);
                    }

                    if (DpadUpWasPressedThisFrame(dpad))
                    {
                        Tap(false);
                    }
                    else if (DpadUpIsPressed(dpad))
                    {
                        Hold(false);
                    }
                }

                // Updating the last known dpad input
                _dpadLastFrame = dpad;
            }
        }
    }

    private void Tap(bool reduce)
    {
        _holdRegisterDelay = holdRegisterDelay;
        float change = gamepadMagnitudeTap;
        ApplyChange(change, reduce);
    }

    private void Hold(bool reduce)
    {
        if (_holdRegisterDelay >= 0)
        {
            return;
        }

        float change = gamepadMagnitudeHold * Time.deltaTime;
        ApplyChange(change, reduce);
    }

    private void ApplyChange(float change, bool reduce)
    {
        change = Mathf.Abs(change);
        if (reduce)
        {
            change *= -1;
        }

        if (inverted)
        {
            change *= -1;
        }

        float newValue = mySlider.value + change;
        print(change);
        newValue = Mathf.Clamp(newValue, mySlider.minValue, mySlider.maxValue);
        mySlider.value = newValue;
    }

    #region dpadDecoding

    //gamepad.leftShoulder.wasPressedThisFrame
    private bool DpadLeftWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadLeftIsPressed(dpad) && _dpadLastFrame.x == 0;
    }

    private bool DpadLeftIsPressed(Vector2Int dpad)
    {
        return dpad.x == -1;
    }

    private bool DpadRightWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadRightIsPressed(dpad) && _dpadLastFrame.x == 0;
    }

    private bool DpadRightIsPressed(Vector2Int dpad)
    {
        return dpad.x == 1;
    }

    private bool DpadUpWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadUpIsPressed(dpad) && _dpadLastFrame.y == 0;
    }

    private bool DpadUpIsPressed(Vector2Int dpad)
    {
        return dpad.y == 1;
    }

    private bool DpadDownWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadDownIsPressed(dpad) && _dpadLastFrame.y == 0;
    }

    private bool DpadDownIsPressed(Vector2Int dpad)
    {
        return dpad.y == -1;
    }

    #endregion
}