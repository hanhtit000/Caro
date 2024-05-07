using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameMenu : Menu
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button[] titleButton;
    public override void Awake()
    {
        base.Awake();
        pauseMenu.SetActive(false);
        resumeButton.onClick.AddListener(Resume);
        foreach (var a in titleButton)
        {
            a.onClick.AddListener(ToTitle);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Resume();
        }
    }

    private void Resume()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);

    }

    private void ToTitle()
    {
        SceneManager.LoadSceneAsync("MenuScene");
    }
}
