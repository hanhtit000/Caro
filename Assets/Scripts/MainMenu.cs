using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button checkButton;
    [SerializeField] private TMP_InputField widthField;
    [SerializeField] private TMP_InputField heightField;
    [SerializeField] private Button botModeButton;

    public override void Awake()
    {
        base.Awake();
        mainMenu.SetActive(true);
        settingMenu.SetActive(false);
        settingButton.onClick.AddListener(Setting);
        returnButton.onClick.AddListener(Setting);
        widthField.text = Width.ToString();
        heightField.text = Height.ToString();
        widthField.onEndEdit.AddListener(WidthEdit);
        heightField.onEndEdit.AddListener(HeightEdit);
        checkButton.GetComponent<Image>().sprite = CurrentTurn? CaroX: CaroO;
        botModeButton.GetComponent<Image>().sprite = BotMode? CheckBotMode: null;
        checkButton.onClick.AddListener(Check);
        botModeButton.onClick.AddListener(EnableBotMode);
    }

    private void Setting()
    {
        settingMenu.SetActive(!settingMenu.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    private void Check()
    {
        if (CurrentTurn == true)
        {
            CurrentTurn = false;
            checkButton.GetComponent<Image>().sprite = CaroO;
        }
        else
        {
            CurrentTurn = true;
            checkButton.GetComponent<Image>().sprite = CaroX;
        }
    }
    private void WidthEdit(string arg0)
    {
        Width = Int32.Parse(widthField.text);
    }

    private void HeightEdit(string arg0)
    {
        Height = Int32.Parse(heightField.text);
    }

    private void EnableBotMode()
    {
        if(BotMode == false)
        {
            BotMode = true;
            botModeButton.GetComponent<Image>().sprite = CheckBotMode;
        }
        else
        {
            BotMode = false;
            botModeButton.GetComponent<Image>().sprite = null;
        }
    }
}
