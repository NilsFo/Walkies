using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private GameState _gameState;
    public CanvasGroup groop;
    public TMP_Text text;

    public float alpha_desired = 0f;
    public float fadeSpeed = 1f;

    public Button playAgainBT;

    // Start is called before the first frame update
    void Start()
    {
        groop.alpha = 0;
        _gameState = FindObjectOfType<GameState>();
        playAgainBT.onClick.AddListener(Again);
    }

    public void Again()
    {
        SceneManager.LoadScene("GameLevel");
    }

    // Update is called once per frame
    void Update()
    {
        groop.alpha = Mathf.MoveTowards(groop.alpha, alpha_desired, Time.deltaTime * fadeSpeed);
        playAgainBT.gameObject.SetActive(groop.alpha > 0);

        text.text = "Territory marked: " + _gameState.InteractionsCount[Interactable.InteractableType.Tree] + "\n" +
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