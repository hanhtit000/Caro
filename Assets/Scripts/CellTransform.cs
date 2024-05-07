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
    private bool isAcitve;
    private int row;
    private int column;

    // Start is called before the first frame update
    private void Awake()
    {
        image = this.GetComponent<Image>();
        this.GetComponent<Button>().onClick.AddListener(Onclick);
        board = FindAnyObjectByType<BoardGenerate>();
        isAcitve = board != null;
    }

    // Update is called once per frame
    public void ChangeImage(int s)
    {
        if(s ==  2)
        {
            image.sprite = CellO;
        }
        else
        {
            image.sprite = CellX;
        }
    }

    public void SetCellLocation(int x, int y)
    {
        row = x; column = y;
    }
    private void Onclick()
    {
        if (!board.isActionable() && isAcitve)
        {
            var turn = board.GetTurn();
            ChangeImage(turn);
            board.Check(row, column);
            if(turn==1) board.SetTurn(turn+1);
            else board.SetTurn(1);
            isAcitve = false;
        }
    }
}
