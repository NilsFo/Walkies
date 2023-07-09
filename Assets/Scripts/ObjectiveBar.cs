using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveBar : MonoBehaviour
{
    public Color colorCharging, frenzyAvailable, freeColor;
    public Slider slider;
    public TextMeshProUGUI textfield;
    public Image fillImage;
    public Image iconImage;

    private GameState _gameState;

    public float fillSpeed = 1.1337f;
    public float fillDesired = 0f;
    public float fillCurrent = 0f;

    public Gradient frenzyDrainGradient;

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fillDesired = 0f;
        fillCurrent = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameState.IsInFrenzyMode())
        {
            textfield.text = "";
            fillDesired = _gameState.GetFrenzyTimeRemainingPercent();
            fillCurrent = fillDesired;
            fillImage.color = frenzyDrainGradient.Evaluate(fillCurrent);
        }
        else if (_gameState.IsInFreeMode())
        {
            textfield.text = "!! FREEDOM !!";
            fillImage.color = freeColor;
            fillDesired = 1f;
            fillCurrent = 1;
        }
        else if (_gameState.FrenzyAvailable())
        {
            textfield.text = "!! SPACEBAR !!";
            fillImage.color = frenzyAvailable;
            fillDesired = 1f;
            fillCurrent = 1;
        }
        else
        {
            fillImage.color = colorCharging;
            textfield.text = ""; //"Frenzy in: " + _gameState.frenzyTokens + "/" + _gameState.frenzyTokenThreshold;
            fillDesired = _gameState.GetFrenzyTokenProgressPercent();

            // if (_gameState.playerSnoot.currentInteractable!=)
            // {
            //     textfield.text = "(E)";
            // }
        }

        if (fillCurrent < fillDesired)
        {
            fillCurrent = fillCurrent + fillSpeed * Time.deltaTime;
            if (fillCurrent > fillDesired)
            {
                fillCurrent = fillDesired;
            }
        }
        else if (fillCurrent > fillDesired)
        {
            fillCurrent = fillCurrent - fillSpeed * Time.deltaTime;
            if (fillCurrent < fillDesired)
            {
                fillCurrent = fillDesired;
            }
        }

        fillCurrent = Math.Clamp(fillCurrent, 0.0f, 1.0f);
        slider.value = fillCurrent;
    }

    private float GetPercentage(int a, int b)
    {
        float af = a;
        float bf = b;
        return GetPercentage(af, bf);
    }

    private float GetPercentage(float a, float b)
    {
        return a / b;
    }
}