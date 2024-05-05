using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CellTransform : MonoBehaviour
{
    [SerializeField] private Sprite CellX;
    [SerializeField] private Sprite CellO;
    private BoardGenerate board;
    private Image image;
    
    // Start is called before the first frame update
    private void Awake()
    {
        image = this.GetComponent<Image>();
        this.GetComponent<Button>().onClick.AddListener(Onclick);
        board = FindAnyObjectByType<BoardGenerate>();
    }

    // Update is called once per frame
    public void ChangeImage(bool s)
    {
        if(s ==  false)
        {
            image.sprite = CellO;
        }
        else
        {
            image.sprite = CellX;
        }
    }

    private void Onclick()
    {
        var turn = board.GetTurn();
        ChangeImage(turn);
        board.SetTurn(!turn);
        Debug.Log(turn);
    }
}
