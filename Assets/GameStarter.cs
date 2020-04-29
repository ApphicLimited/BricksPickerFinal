﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public static GameStarter instance;

    public int currentLevel = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != instance)
            Destroy(this);

        if (PlayerPrefs.HasKey("LevelId"))
        {
            currentLevel = PlayerPrefs.GetInt("LevelId");
        }
        else
        {
            PlayerPrefs.SetInt("LevelId", currentLevel);
        }
        Debug.LogError(currentLevel);
        Instantiate(Resources.Load("Level" + currentLevel), new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void LoadNextLevel()
    {
        LevelPassed();
        SceneManager.LoadScene("GameScene");
    }

    public void LevelPassed()
    {
        currentLevel++;
        PlayerPrefs.SetInt("LevelId", currentLevel);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Fail()
    {
        GameManager.instance.RestartButton.SetActive(true);
    }
}
