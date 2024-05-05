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
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private bool turnX;

    // Start is called before the first frame update
    void Start()
    {
        var board = this.GetComponent<GridLayoutGroup>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++)
            {
                Instantiate(prefab,this.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Console.Write(Input.GetAxis("Mouse ScrollWheel"));
        var board = this.GetComponent<GridLayoutGroup>(); 
        board.cellSize.Set(board.cellSize.x+Input.GetAxis("Mouse ScrollWheel")*10, board.cellSize.y + Input.GetAxis("Mouse ScrollWheel")*10);
    }

    public bool GetTurn()
    {
        return turnX;
    }
    public void SetTurn(bool turn)
    {
        turnX = !turn;
    }
}
