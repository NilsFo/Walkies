using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private GameState _gameState;
    public CanvasGroup groop;
    public TMP_Text text;

    public float alpha_desired = 0f;
    public float fadeSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        groop.alpha = 0;
        _gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        groop.alpha = Mathf.MoveTowards(groop.alpha, alpha_desired, Time.deltaTime * fadeSpeed);

        text.text = "Territory defended: " + _gameState.InteractionsCount[Interactable.InteractableType.Tree] + "\n" +
                    "Friends made: " + _gameState.InteractionsCount[Interactable.InteractableType.Friend] + "\n" +
                    "Pets get: " + _gameState.InteractionsCount[Interactable.InteractableType.Pet] + "\n" +
                    "Zoomies: " + _gameState.frenzyCount
            ;
    }

    public void StartFade()
    {
        alpha_desired = 1;
    }
}