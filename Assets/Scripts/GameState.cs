using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

public class GameState : MonoBehaviour
{
    [Header("Gameplay")] public int targetFPS = 60;

    [Header("Walking Path")] public OwnerAI ownerAI;
    public OwnerPath ownerPath;
    public int ownerTargetWaypointIndex;
    public float waypointReachedDistance = 0.42f;

    [Header("Player")] public PlayerMovementBehaviour player;
    public DogSnoot playerSnoot;
    public MusicManager musicManager;
    public GameOverUI gameOverUI;
    public TMP_Text bonesTF;
    public Slider volumeSlider;

    [Header("Interactions")] public Dictionary<Interactable.InteractableType, int> InteractionsCount;

    [Header("FrenzyCounter")] public int frenzyTokenThreshold = 2;
    public int frenzyTokens = 0;
    public float frenzySpeedMult = 2.0f;

    public float frenzyTime = 5;
    public float subsequentialAccumulativeFrenzyTimeSeconds = 3f;
    public float maximumAccumulativeFrenzyTimeSeconds = 10f;
    public float frenzyTimeCurrent = 0;
    public bool playerMovedDuringFrenzy = false;
    public int frenzyCount = 0;
    public AudioClip ropePull;

    [Header("Camera")] public CinemachineVirtualCamera virtualCamera;
    public float cameraDistanceDefault = 5f;
    public float cameraDistanceFrenzy = 7f;
    public float cameraDistanceFreedom = 10f;
    public float cameraDistanceInitial = 8f;
    public float zoomSpeed = 2f;

    [Header("Bones")] public int bonesCollectedCount = 0;
    public int bonesCollectedTarget = 0;

    [Header("Winning")] public GameObject invisibleWallsHolder;
    public GameObject winTriggerHolder;
    public float autoWinTimerCurrent = 0;
    public float autoWinTimer = 30;
    public bool hasAutoWon = false;

    [Header("StartGame")] public CanvasGroup startGameCanvas;
    public float startGameCanvasAlphaDesired = 1.0f;
    public float startGameCanvasFadeSpeed = 2f;
    public Button startGameBT;
    public Button dragonaBT;
    public GameObject objectiveBarHolder;
    public GameObject startGameCameraFollow;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFPS;
        gameOverUI.gameObject.SetActive(true);
        hasAutoWon = false;

        InteractionsCount = new Dictionary<Interactable.InteractableType, int>
        {
            { Interactable.InteractableType.Friend, 0 },
            { Interactable.InteractableType.Pet, 0 },
            { Interactable.InteractableType.Treat, 0 },
            { Interactable.InteractableType.Tree, 0 }
        };

        ownerTargetWaypointIndex = -1;
        StartWalkies();

        musicManager.Play(0);
        musicManager.SkipFade();

        // Updating bones objective
        var bones = FindObjectsOfType<CollectibleBone>();
        bonesCollectedTarget = bones.Length;

        invisibleWallsHolder.SetActive(true);
        winTriggerHolder.SetActive(false);
        startGameBT.onClick.AddListener(StartGame);
        dragonaBT.onClick.AddListener(BrowseDragonaSpotifyURL);

        player.rb2D.simulated = false;
        ownerAI.rb2D.simulated = false;
        virtualCamera.Follow = startGameCameraFollow.transform;
    }

    public void StartWalkies()
    {
        ownerTargetWaypointIndex = 0;
    }

    public void BrowseDragonaSpotifyURL()
    {
        try
        {
            Application.OpenURL("https://open.spotify.com/artist/42x9vvb7JdMnkUHb26Z89Z?si=3ysfJw7JTm6QM_y9LChcXg");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Debug.LogError("URL open error!");
        }
    }

    public void FinishWalkies()
    {
    }

    public void StartGame()
    {
        startGameCanvasAlphaDesired = 0.0f;
        player.rb2D.simulated = true;
        ownerAI.rb2D.simulated = true;

        virtualCamera.Follow = FindObjectOfType<LineConnectionSprite>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        startGameCanvas.alpha = Mathf.MoveTowards(startGameCanvas.alpha, startGameCanvasAlphaDesired,
            Time.deltaTime * startGameCanvasFadeSpeed);
        if (startGameCanvas.alpha <= 0)
        {
            startGameCanvas.gameObject.SetActive(false);
            objectiveBarHolder.SetActive(true);
        }
        else
        {
            objectiveBarHolder.SetActive(false);
        }

        if (IsInFrenzyMode())
        {
            frenzyTimeCurrent -= Time.deltaTime;
            if (frenzyTimeCurrent <= 0)
            {
                OnExitFrenzyMode();
            }
        }
        else
        {
            frenzyTimeCurrent = 0;
            playerMovedDuringFrenzy = false;
        }

        if (IsInFreeMode())
        {
            autoWinTimerCurrent = autoWinTimerCurrent += Time.deltaTime;
            if (autoWinTimerCurrent >= autoWinTimer)
            {
                Win();
                hasAutoWon = true;
            }
        }
        else
        {
            autoWinTimerCurrent = 0;
        }

        // Updating bonesTF text
        bonesTF.text = bonesCollectedCount + "/" + bonesCollectedTarget;

        MusicManager.userDesiredMusicVolume = volumeSlider.value;
        MusicManager.userDesiredSoundVolume = volumeSlider.value;
        if (!HasGameStartedYet())
        {
            musicManager.SkipFade();
        }
    }

    public bool HasGameStartedYet()
    {
        return objectiveBarHolder.activeSelf;
    }

    private void LateUpdate()
    {
        float currentOrthographicDistance = virtualCamera.m_Lens.OrthographicSize;
        float desiredOrthographicDistance = cameraDistanceDefault;

        if (!HasGameStartedYet())
        {
            desiredOrthographicDistance = cameraDistanceInitial;
        }

        if (IsInFrenzyMode())
        {
            desiredOrthographicDistance = cameraDistanceFrenzy;
        }

        if (IsInFreeMode())
        {
            desiredOrthographicDistance = cameraDistanceFreedom;
        }

        currentOrthographicDistance = Mathf.MoveTowards(currentOrthographicDistance, desiredOrthographicDistance,
            Time.deltaTime * zoomSpeed);
        virtualCamera.m_Lens.OrthographicSize = currentOrthographicDistance;
    }

    public void EnterFrenzyMode()
    {
        frenzyTimeCurrent = frenzyTime;
        frenzyTokens = 0;
        playerSnoot.currentInteractable = null;
        OnEnterFrenzyMode(true, true);
    }

    public void OnEnterFrenzyMode(bool incrementCounter, bool playAudio)
    {
        musicManager.Play(1, true);
        musicManager.SkipFade();

        if (incrementCounter)
        {
            frenzyCount++;
        }

        if (playAudio)
        {
            musicManager.CreateAudioClip(ropePull, transform.position);
        }
        
        if (bonesCollectedCount >= bonesCollectedTarget - 5)
        {
            var kompass = FindObjectOfType<KompassBehaviourScript>();
            kompass.SetKompassVisible(true);
        }
    }

    public void OnExitFrenzyMode()
    {
        print("OnFrenzyExit!");
        if (!IsInFreeMode())
        {
            // Only switch music when not in freedom
            musicManager.Play(0);
        }

        // updating time for the next frenzy
        frenzyTime = frenzyTime + subsequentialAccumulativeFrenzyTimeSeconds;
        frenzyTime = Mathf.Min(frenzyTime, maximumAccumulativeFrenzyTimeSeconds);

        // Set closest waypoint
        int closestIndex = ownerPath.waypoints.IndexOf(ownerPath.waypoints.OrderBy(waypoint =>
            Vector2.Distance(waypoint.transform.position, ownerAI.transform.position)).FirstOrDefault());
        ownerTargetWaypointIndex = closestIndex;
        
        var kompass = FindObjectOfType<KompassBehaviourScript>();
        kompass.SetKompassVisible(false);
    }

    public bool IsInFrenzyMode()
    {
        return frenzyTimeCurrent > 0;
    }

    public bool IsInFreeMode()
    {
        return player.currentInputState == PlayerMovementBehaviour.PlayerInputState.Free;
    }

    public bool FrenzyAvailable()
    {
        return frenzyTokens >= frenzyTokenThreshold && !IsInFreeMode();
    }

    public void AddFrenzyPoint()
    {
        frenzyTokens += 1;
    }

    public float GetFrenzyTimeRemainingPercent()
    {
        return frenzyTimeCurrent / frenzyTime;
    }

    public float GetFrenzyTokenProgressPercent()
    {
        return (float)frenzyTokens / (float)frenzyTokenThreshold;
    }

    public void OnBoneCollected()
    {
        bonesCollectedCount++;
        if (bonesCollectedCount >= bonesCollectedTarget && !IsInFreeMode())
        {
            player.BreakFree();
        }

        if (bonesCollectedCount >= bonesCollectedTarget - 5)
        {
            var kompass = FindObjectOfType<KompassBehaviourScript>();
            kompass.UpdateList(FindObjectsOfType<CollectibleBone>().Where(b => !b.IsDeleted()).ToArray());
        }
    }

    public void OnWaypointReached(int index)
    {
        ownerTargetWaypointIndex = index + 1;
        if (ownerTargetWaypointIndex >= ownerPath.waypoints.Count)
        {
            print("WALKIES OVER! Restarting!");
            ownerTargetWaypointIndex = 0;
        }

        ownerAI.fastWalk = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Win()
    {
        if (hasAutoWon)
        {
            return;
        }

        hasAutoWon = true;
        Debug.Log("u win");
        gameOverUI.StartFade();
        musicManager.Play(2);
    }
}