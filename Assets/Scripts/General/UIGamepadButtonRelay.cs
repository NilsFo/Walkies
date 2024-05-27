using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class UIGamepadButtonRelay : MonoBehaviour
{
    private GamepadInputDetector _gamepadInputDetector;

    [Header("Hookup")] public Button myButton;
    private EventSystem _eventSystem;

    [Header("Gamepad Button to listen for")] public bool north;
    public bool south;
    public bool west;
    public bool east;
    public bool select;
    public bool start;

    private void Awake()
    {
        _gamepadInputDetector = FindObjectOfType<GamepadInputDetector>();
        _eventSystem = FindObjectOfType<EventSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (north && gamepad.buttonNorth.wasPressedThisFrame)
                {
                    Trigger();
                }

                if (south && gamepad.buttonSouth.wasPressedThisFrame)
                {
                    Trigger();
                }

                if (west && gamepad.buttonWest.wasPressedThisFrame)
                {
                    Trigger();
                }

                if (east && gamepad.buttonEast.wasPressedThisFrame)
                {
                    Trigger();
                }
                if (select && gamepad.selectButton.wasPressedThisFrame)
                {
                    Trigger();
                }
                if (start && gamepad.startButton.wasPressedThisFrame)
                {
                    Trigger();
                }
            }
        }
    }

    private void Trigger()
    {
        ExecuteEvents.Execute(myButton.gameObject, new BaseEventData(_eventSystem), ExecuteEvents.submitHandler);
    }
}