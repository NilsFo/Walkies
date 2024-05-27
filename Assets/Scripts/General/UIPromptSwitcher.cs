using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPromptSwitcher : MonoBehaviour
{

    private GamepadInputDetector _gamepadInputDetector;

    public GameObject visibleKBM;
    public GameObject visibleGamePad;
    
    private void Awake()
    {
        _gamepadInputDetector = FindObjectOfType<GamepadInputDetector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (visibleGamePad != null)
        {
            visibleGamePad.SetActive(_gamepadInputDetector.isGamePad);
        }
        
        if (visibleKBM != null)
        {
            visibleKBM.SetActive(!_gamepadInputDetector.isGamePad);
        }
    }
}
