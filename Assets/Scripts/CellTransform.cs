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
    private Image image;
    private bool isAcitve;
    private int row;
    private int column;

    // Start is called before the first frame update
    private void Awake()
    {
        image = this.GetComponent<Image>();
        this.GetComponent<Button>().onClick.AddListener(Onclick);
        isAcitve = BoardGenerate.Instance != null;
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

    public void SetCellLocation(int x, int y)
    {
        row = x; column = y;
    }
    private void Onclick()
    {
        var board = BoardGenerate.Instance;
        if (!board.isActionable() && isAcitve)
        {
            var turn = board.GetTurn();
            ChangeImage(turn);
            board.Check(row, column);
            board.SetTurn();
            isAcitve = false;
        }
    }

    public void BotClick()
    {
        Onclick();
    }
}
