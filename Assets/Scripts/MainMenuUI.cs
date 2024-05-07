using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject exit;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button checkButton;
    [SerializeField] private TMP_InputField widthField;
    [SerializeField] private TMP_InputField heightField;
    [SerializeField] private Sprite caroX;
    [SerializeField] private Sprite caroO;
    private int width;
    private int height;
    private int currentTurn;

    // Start is called before the first frame update
    private void Awake()
    {
        mainMenu.SetActive(true);
        settingMenu.SetActive(false);
        playButton.onClick.AddListener(PlayGame);
        settingButton.onClick.AddListener(Setting);
        returnButton.onClick.AddListener(Setting);
        exitButton.onClick.AddListener(Exit);
        checkButton.onClick.AddListener(Check);
        width = 10;
        height = 10;
        currentTurn = 1;
        widthField.text = width.ToString();
        heightField.text = height.ToString();
        widthField.onEndEdit.AddListener(WidthEdit);
        heightField.onEndEdit.AddListener(HeightEdit);
        checkButton.GetComponent<Image>().sprite = caroX;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayGame()
    {
        StaticData.width = width;
        StaticData.height = height;
        StaticData.currentTurn = currentTurn;
        SceneManager.LoadSceneAsync("GameScene");
    }

    private void Setting()
    {
        settingMenu.SetActive(!settingMenu.activeSelf);
        if(mainMenu != null) mainMenu.SetActive(!mainMenu.activeSelf);
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void Check()
    {
        if(currentTurn == 1)
        {
            currentTurn = 2;
            checkButton.GetComponent<Image>().sprite = caroO;
        }
        else
        {
            currentTurn = 1;
            checkButton.GetComponent<Image>().sprite = caroX;
        }
    }
    private void WidthEdit(string arg0)
    {
        width = Int32.Parse(widthField.text);
    }

    private void HeightEdit(string arg0)
    {
        height = Int32.Parse(heightField.text);
    }
}
