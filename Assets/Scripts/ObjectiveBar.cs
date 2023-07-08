using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveBar : MonoBehaviour
{
    public Color colorCharging, frenzyAvailable, frenzyDrain;
    public Slider slider;
    public TextMeshProUGUI textfield;
    public Image fillImage;
    public Image iconImage;

    private GameState _gameState;

    public float fillSpeed = 1.1337f;
    public float fillDesired = 0f;
    public float fillCurrent = 0f;

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
            fillImage.color = frenzyDrain;
            fillDesired = _gameState.GetFrenzyTimeRemainingPercent();
            fillCurrent = fillDesired;
        }
        else if (_gameState.FrenzyAvailable())
        {
            textfield.text = "[SPACEBAR]";
            fillImage.color = frenzyAvailable;
            fillDesired = 1f;
        }
        else
        {
            textfield.text = "Frenzy in: " + _gameState.frenzyTokens + "/" + _gameState.frenzyTokenThreshold;
            fillDesired = _gameState.GetFrenzyTokenProgressPercent();
        }

        fillCurrent = Mathf.Lerp(fillCurrent, fillDesired, Time.deltaTime);
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