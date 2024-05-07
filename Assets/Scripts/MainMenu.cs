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
        checkButton.GetComponent<Image>().sprite = CaroX;
        checkButton.onClick.AddListener(Check);
    }

    private void Setting()
    {
        settingMenu.SetActive(!settingMenu.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    private void Check()
    {
        if (CurrentTurn == 1)
        {
            CurrentTurn = 2;
            checkButton.GetComponent<Image>().sprite = CaroO;
        }
        else
        {
            CurrentTurn = 1;
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
}
