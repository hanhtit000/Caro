using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class BoardGenerate : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private int height;
    private int width;
    private int currentTurn;
    private int[,] caroMatrix;
    private bool isWin;

    // Start is called before the first frame update
    void Start()
    {
        width = StaticData.width;
        height = StaticData.height;
        currentTurn = StaticData.currentTurn;
        CreateBoard(width, height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Zoom()
    {
        var board = this.GetComponent<GridLayoutGroup>();
        Console.Write(Input.GetAxis("Mouse ScrollWheel"));
        board.cellSize.Set(board.cellSize.x + Input.GetAxis("Mouse ScrollWheel") * 10, board.cellSize.y + Input.GetAxis("Mouse ScrollWheel") * 10);
    }

    private void CreateBoard(int width, int height)
    {
        caroMatrix = new int[height + 1, width + 1];
        isWin = false;
        var board = this.GetComponent<GridLayoutGroup>();
        board.constraintCount = height;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Instantiate(prefab, this.transform).GetComponent<CellTransform>().SetCellLocation(i,j);
            }
        }
    }
    private void Win()
    {
        isWin = true;
        if(currentTurn==1)
        {

            Debug.Log("X win");
        }
        else
        {
            Debug.Log("O win");
        }
        
    }

    public void Check(int row, int column)
    {
        int dem = 0;
        caroMatrix[row, column] = currentTurn;
        Debug.Log(row+" "+ column);
        //Check Row
        for(int i = row-1; i >= 0; i--)
        {
            if (caroMatrix[i, column] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        for (int i = row+1; i < height; ++i)
        {
            if (caroMatrix[i, column] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        if(dem >=4)
        {
            Win();
            return;
        }

        //Check Column
        dem = 0;
        for (int i = column - 1; i >=0; i--)
        {
            if (caroMatrix[row, i] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        for (int i = column+1; i < width; ++i)
        {
            if (caroMatrix[row, i] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        if (dem >= 4)
        {
            Win();
            return;
        }

        //Check main diagonal
        dem = 0;
        for (int i = 1; i <= Math.Min(row, column); i++)
        {
            if (caroMatrix[row-i, column-i] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        for (int i = 1; i < Math.Min(height-row, width-column); i++)
        {
            if (caroMatrix[row + i, column + i] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        if (dem >= 4)
        {
            Win();
            return;
        }

        //Check sub diagonal
        dem = 0;
        for (int i = 1; i <= Math.Min(row, column); i++)
        {
            if (caroMatrix[row - i, column + i] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        for (int i = 1; i < Math.Min(height - row, width - column); i++)
        {
            if (caroMatrix[row + i, column - i] == currentTurn)
            {
                dem++;
            }
            else break;
        }
        if (dem >= 4)
        {
            Win();
            return;
        }
    }
    public int GetTurn()
    {
        return currentTurn;
    }
    public void SetTurn(int turn)
    {
        currentTurn = turn;
    }

    public bool isActionable()
    {
        return isWin;
    }
}
