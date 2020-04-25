using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("MANAGERS")]
    public PlayerManager PlayerManager;
    public ObstacleManager ObstacleManager;
    public StackManager StackManager;
    public AudioManager AudioManager;
    public PanelManager PanelManager;
    [Header("CONTROLLERS")]
    public TouchController TouchController;
    public SuperPowerController SuperPowerController;
    public CongratWordsController CongratWordsController;
    public ColourController ColourController;
    public ScoreController ScoreController;
    public CoinController CoinController;
    public TimeController TimeController;

    public SmoothFollow SmothFollow;

    public GameStates GameState;
    public bool IsGameStarted;
    public bool IsGameDone;

    public event Action OnGameStarted;
    public event Action OnGameDone;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != instance)
            Destroy(this);
    }

    private void Start()
    {
        IsGameStarted = false;
    }

    private void Update()
    {
        if (GameState != GameStates.GameOnGoing)
            return;

        if (IsGameStarted)
        {
            OnGameStarted?.Invoke();
            IsGameStarted = false;
        }
        else if (IsGameDone)
        {
            OnGameDone?.Invoke();
            IsGameDone = false;
        }
    }
}