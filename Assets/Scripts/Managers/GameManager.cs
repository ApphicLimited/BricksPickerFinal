﻿using System;
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
    public Material MetreDetecterMaterial;

    private bool isGameStarted;
    public bool IsGameStarted
    {
        get { return IsGameStarted; }
        set
        {
            isGameStarted = value;
            if (isGameStarted)
                OnGameStarted?.Invoke();
        }
    }
    private bool isGameDone { get; set; }
    public bool IsGameDone
    {
        get
        { return isGameDone; }
        set
        {
            isGameDone = value;
            if (IsGameDone)
                OnGameDone?.Invoke();
        }
    }

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

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        IsGameStarted = false;
    }
}