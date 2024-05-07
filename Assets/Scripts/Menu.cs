using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button[] playButton;
    [SerializeField] private Button[] exitButton;
    [SerializeField] private Sprite caroX;
    [SerializeField] private Sprite caroO;
    private int width;
    private int height;
    private int currentTurn;

    public Sprite CaroX { get { return caroX; } }
    public Sprite CaroO { get { return caroO; } }
    public int Width
    {
        get { return width; }
        set { width = value; }
    }

    public int Height
    {
        get { return height; }
        set { height = value; }
    }
    public int CurrentTurn
    {
        get { return currentTurn; }
        set { currentTurn = value; }
    }
    // Start is called before the first frame update
    public virtual void Awake()
    {
        foreach(var a in playButton) 
        {
            a.onClick.AddListener(PlayGame);
        }
        foreach (var a in exitButton)
        {
            a.onClick.AddListener(Exit);
        }
        width = 10;
        height = 10;
        currentTurn = 1;
    }
    private void PlayGame()
    {
        StaticData.width = Width;
        StaticData.height = Height;
        StaticData.currentTurn = CurrentTurn;
        SceneManager.LoadSceneAsync("GameScene");
    }
    private void Exit()
    {
        Application.Quit();
    }
}
