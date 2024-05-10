using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class BoardGenerate : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI winText;
    private int height;
    private int width;
    private bool currentTurn;
    private int[,] caroMatrix;
    private bool isWin;
    private bool isBotMode;
    private int turnLeft;
    private bool playerTurn;
    private bool botTurn;
    private List<CellTransform> caroList;

    // Start is called before the first frame update
    void Start()
    {
        gameOverUI.SetActive(false);
        width = StaticData.width;
        height = StaticData.height;
        currentTurn = StaticData.currentTurn;
        if (StaticData.botMode)
        {
            isBotMode = StaticData.botMode;
            playerTurn = currentTurn;
            botTurn = !currentTurn;
        }

        turnLeft = width * height;
        CreateBoard(width, height);
    }

    // Update is called once per frame
    void Update()
    {
        Zoom();
        ChangePosition();
    }

    private void ChangePosition()
    {
        var speed = 100000;
        GetComponent<RectTransform>().Translate(Vector3.up * Time.deltaTime * Input.GetAxis("Vertical") * Time.deltaTime * speed);
        GetComponent<RectTransform>().Translate(Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal") * Time.deltaTime * speed);
    }

    private void Zoom()
    {
        var board = this.GetComponent<GridLayoutGroup>();
        var zoom = Input.GetAxis("Mouse ScrollWheel") * 100;
        board.cellSize = new Vector2(board.cellSize.x + zoom, board.cellSize.y + zoom);
    }

    private void CreateBoard(int width, int height)
    {
        caroMatrix = new int[height + 1, width + 1];
        isWin = false;
        var board = this.GetComponent<GridLayoutGroup>();
        caroList = new List<CellTransform>();
        board.constraintCount = height;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var cell = Instantiate(prefab, this.transform).GetComponent<CellTransform>();
                cell.SetCellLocation(i, j);
                caroList.Add(cell);
            }
        }
    }
    private void Win()
    {
        isWin = true;
        gameOverUI.SetActive(true);
        if (turnLeft != 0)
        {
            if (currentTurn == true)
            {
                winText.text = "X win";
            }
            else
            {
                winText.text = "O win";
            }
        }
        else
        {
            winText.text = "Draw";
        }

    }

    public bool GetTurn()
    {
        return currentTurn;
    }
    public void SetTurn()
    {
        currentTurn = !currentTurn;
        if (isBotMode)
        {
            if (currentTurn == botTurn) BotCheck();
        }
    }

    public bool isActionable()
    {
        return isWin;
    }

    public void BotCheck()
    {
        int maxPoint = 0;
        int defPoint = 0;
        int atkPoint = 0;
        Vector2Int location = Vector2Int.zero;
        //thuật toán minmax tìm điểm cao nhất để đánh
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //nếu nước cờ chưa có ai đánh và không bị cắt tỉa thì mới xét giá trị MinMax
                if (caroMatrix[i, j] == 0 && !CheckEnemies(i, j))
                {
                    int DiemTam;

                    atkPoint = CheckAtkRow(i, j) + CheckAtkColumn(i, j) + CheckAtkMainDiagonal(i, j) + CheckAtkSubDiagonal(i, j);
                    defPoint = CheckDefRow(i, j) + CheckDefColumn(i, j) + CheckDefMainDiagonal(i, j) + CheckDefSubDiagonal(i, j);

                    if (defPoint > atkPoint)
                    {
                        DiemTam = defPoint;
                    }
                    else
                    {
                        DiemTam = atkPoint;
                    }

                    if (maxPoint < DiemTam)
                    {
                        maxPoint = DiemTam;
                        location = new Vector2Int(i, j);
                    }
                }
            }
        }
        int turn = currentTurn ? 2 : 1;
        caroMatrix[location.x,location.y] = turn;
        caroList[location.x * width + location.y].BotClick();
    }


    #region Cắt tỉa Alpha betal
    bool CheckEnemies(int row, int column)
    {
        //nếu cả 4 hướng đều không có nước cờ thì cắt tỉa
        if (CheckEnemiesRow(row, column) && CheckEnemiesColumn(row, column) &&
            CheckEnemiesMainDiagonal(row, column) && CheckEnemiesSubDiagonal(row, column)) return true;
        //chạy đến đây thì 1 trong 4 hướng vẫn có nước cờ thì không được cắt tỉa
        return false;
    }

    bool CheckEnemiesRow(int row, int column)
    {
        //duyệt bên phải
        if (column <= width - 5)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row, column + i] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //duyệt bên trái
        if (column >= 4)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row, column - i] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
        return true;
    }
    bool CheckEnemiesColumn(int row, int column)
    {
        //duyệt phía giưới
        if (row <= height - 5)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row + i, column] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //duyệt phía trên
        if (row >= 4)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row - i, column] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
        return true;
    }
    bool CheckEnemiesMainDiagonal(int row, int column)
    {
        //duyệt từ trên xuống
        if (row <= height - 5 && column >= 4)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row + i, column - i] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //duyệt từ giưới lên
        if (column <= width - 5 && row >= 4)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row - i, column + i] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
        return true;
    }
    bool CheckEnemiesSubDiagonal(int row, int column)
    {
        //duyệt từ trên xuống
        if (row <= height - 5 && column <= width - 5)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row + i, column + i] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //duyệt từ giưới lên
        if (column >= 4 && row >= 4)
            for (int i = 1; i <= 4; i++)
                if (caroMatrix[row - i, column - i] != 0)//nếu có nước cờ thì không cắt tỉa
                    return false;

        //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
        return true;
    }

    #endregion

    #region AI

    private int[] listAtkPoint = new int[7] { 0, 3, 24, 243, 2197, 19773, 177957 };
    private int[] listDefPoint = new int[7] { 0, 4, 25, 246, 7300, 6561, 59049 };

    #region ATTACK
    //duyệt ngang
    public int CheckAtkRow(int row, int column)
    {
        int atkPoint = 0;
        int allies = 0;
        int rightEnemies = 0;
        int leftEnemies = 0;
        int space = 0;
        int turn = botTurn ? 2 : 1;
        //bên phải
        for (int count = 1; count <= 4 && column < width - 5; count++)
        {
            if (caroMatrix[row, column + count] == turn)
            {
                if (count == 1) atkPoint += 37;
                allies++;
                space++;
            }
            else
            {
                if (caroMatrix[row, column + count] == 0) space++;
                else
                {
                    rightEnemies++;
                    break;
                }
            }
        }

        //bên trái
        for (int count = 1; count <= 4 && column > 4; count++)
        {
            if (caroMatrix[row, column - count] == 1)
            {
                if (count == 1) atkPoint += 37;
                allies++;
                space++;
            }
            else
            {
                if (caroMatrix[row, column - count] == 0) space++;
                else
                {
                    leftEnemies++;
                    break;
                }
            }
        }
        //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
        if (rightEnemies > 0 && leftEnemies > 0 && space < 4) return 0;
        atkPoint -= listDefPoint[rightEnemies + leftEnemies];
        atkPoint += listAtkPoint[allies];
        return atkPoint;
    }

    //duyệt dọc
    public int CheckAtkColumn(int row, int column)
    {
        int atkPoint = 0;
        int allies = 0;
        int upEnemies = 0;
        int downEnemies = 0;
        int space = 0;
        int turn = botTurn ? 2 : 1;
        //bên trên
        for (int count = 1; count <= 4 && row > 4; count++)
        {
            if (caroMatrix[row - count, column] == turn)
            {
                if (count == 1) atkPoint += 37;
                allies++;
                space++;
            }
            else
            {
                if (caroMatrix[row - count, column] == 0) space++;
                else
                {
                    upEnemies++;
                    break;
                }
            }

        }
        //bên dưới
        for (int count = 1; count <= 4 && row < height - 5; count++)
        {
            if (caroMatrix[row + count, column] == turn)
            {
                if (count == 1) atkPoint += 37;
                allies++;
                space++;
            }
            else
            {
                if (caroMatrix[row + count, column] == 0) space++;
                else
                {
                    downEnemies++;
                    break;
                }
            }
        }
        //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
        if (upEnemies > 0 && downEnemies > 0 && space < 4) return 0;
        atkPoint -= listDefPoint[upEnemies + downEnemies];
        atkPoint += listAtkPoint[allies];
        return atkPoint;
    }

    //chéo xuôi
    public int CheckAtkMainDiagonal(int row, int column)
    {
        int atkPoint = 1;
        int allies = 0;
        int upDiagonalEnemies = 0;
        int downDiagonalEnemies = 0;
        int space = 0;
        int turn = botTurn ? 2 : 1;
        //bên chéo xuôi xuống
        for (int count = 1; count <= 4 && column < width - 5 && row < height - 5; count++)
        {
            if (caroMatrix[row + count, column + count] == turn)
            {
                if (count == 1) atkPoint += 37;
                allies++;
                space++;

            }
            else
            {
                if (caroMatrix[row + count, column + count] == 0) space++;
                else
                {
                    downDiagonalEnemies++;
                    break;
                }
            }
        }
        //chéo xuôi lên
        for (int count = 1; count <= 4 && row > 4 && column > 4; count++)
        {
            if (caroMatrix[row - count, column - count] == turn)
            {
                if (count == 1) atkPoint += 37;
                allies++;
                space++;
            }
            else
            {
                if (caroMatrix[row - count, column - count] == 0) space++;
                else
                {
                    upDiagonalEnemies++;
                    break;
                }
            }
        }
        //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
        if (upDiagonalEnemies > 0 && downDiagonalEnemies > 0 && space < 4) return 0;
        atkPoint -= listDefPoint[upDiagonalEnemies + downDiagonalEnemies];
        atkPoint += listAtkPoint[allies];
        return atkPoint;
    }

    //chéo ngược
    public int CheckAtkSubDiagonal(int row, int column)
    {
        int atkPoint = 0;
        int allies = 0;
        int upDiagonalEnemies = 0;
        int downDiagonalEnemies = 0;
        int space = 0;
        int turn = botTurn ? 2 : 1;
        //chéo ngược lên
        for (int count = 1; count <= 4 && column < width - 5 && row > 4; count++)
        {
            if (caroMatrix[row - count, column + count] == turn)
            {
                if (count == 1) atkPoint += 37;
                allies++;
                space++;
            }
            else
            {
                if (caroMatrix[row - count, column + count] == 0) space++;
                else
                {
                    upDiagonalEnemies++;
                    break;
                }
            }
        }
        //chéo ngược xuống
        for (int count = 1; count <= 4 && column > 4 && row < height - 5; count++)
        {
            if (caroMatrix[row + count, column - count] == turn)
            {
                if (count == 1) atkPoint += 37;

                allies++;
                space++;

            }
            else
            {
                if (caroMatrix[row + count, column - count] == 0) space++;
                else
                {
                    downDiagonalEnemies++;
                    break;
                }
            }
        }
        //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
        if (upDiagonalEnemies > 0 && downDiagonalEnemies > 0 && space < 4) return 0;
        atkPoint -= listDefPoint[upDiagonalEnemies + downDiagonalEnemies];
        atkPoint += listAtkPoint[allies];
        return atkPoint;
    }
    #endregion

    #region DEFENSE

    //duyệt ngang
    public int CheckDefRow(int row, int column)
    {
        int defPoint = 0;
        int leftAllies = 0;
        int rightAllies = 0;
        int enemies = 0;
        int rightSpace = 0;
        int leftSpace = 0;
        bool ok = false;
        int turn = botTurn ? 2 : 1;

        for (int count = 1; count <= 4 && column < width - 5; count++)
        {
            if (caroMatrix[row, column + count] == turn)
            {
                if (count == 4) defPoint -= 170;
                leftAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row, column + count] == 0)
                {
                    if (count == 1) ok = true;
                    rightSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }
        }
        if (enemies == 3 && rightSpace == 1 && ok) defPoint -= 200;
        ok = false;
        for (int count = 1; count <= 4 && column > 4; count++)
        {
            if (caroMatrix[row, column - count] == turn)
            {
                if (count == 4) defPoint -= 170;
                rightAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row, column - count] == 0)
                {
                    if (count == 1) ok = true;
                    leftSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }
        }
        if (enemies == 3 && leftSpace == 1 && ok) defPoint -= 200;
        if (rightAllies > 0 && leftAllies > 0 && (leftSpace + rightSpace + enemies) < 4) return 0;
        defPoint -= listAtkPoint[rightAllies + rightAllies];
        defPoint += listDefPoint[enemies];
        return defPoint;
    }

    //duyệt dọc
    public int CheckDefColumn(int row, int column)
    {
        int defPoint = 0;
        int upAllies = 0;
        int downAllies = 0;
        int enemies = 0;
        int upSpace = 0;
        int downSpace = 0;
        bool ok = false;
        int turn = botTurn ? 2 : 1;
        //lên
        for (int count = 1; count <= 4 && row > 4; count++)
        {
            if (caroMatrix[row - count, column] == turn)
            {
                if (count == 4) defPoint -= 170;
                downAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row - count, column] == 0)
                {
                    if (count == 1) ok = true;
                    upSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }
        }
        if (enemies == 3 && upSpace == 1 && ok) defPoint -= 200;
        ok = false;

        //xuống
        for (int count = 1; count <= 4 && row < height - 5; count++)
        {
            //gặp quân địch
            if (caroMatrix[row + count, column] == turn)
            {
                if (count == 4) defPoint -= 170;
                upAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row + count, column] == 0)
                {
                    if (count == 1) ok = true;
                    downSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }
        }

        if (enemies == 3 && downSpace == 1 && ok) defPoint -= 200;
        if (downAllies > 0 && upAllies > 0 && (upSpace + downSpace + enemies) < 4) return 0;
        defPoint -= listAtkPoint[upAllies + downAllies];
        defPoint += listDefPoint[enemies];
        return defPoint;
    }

    //chéo xuôi
    public int CheckDefMainDiagonal(int row, int column)
    {
        int defPoint = 0;
        int leftAllies = 0;
        int rightAllies = 0;
        int enemies = 0;
        int upSpace = 0;
        int downSpace = 0;
        bool ok = false;
        int turn = botTurn ? 2 : 1;

        //lên
        for (int count = 1; count <= 4 && row < height - 5 && column < width - 5; count++)
        {
            if (caroMatrix[row + count, column + count] == turn)
            {
                if (count == 4) defPoint -= 170;
                rightAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row + count, column + count] == 0)
                {
                    if (count == 1) ok = true;
                    upSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }
        }
        if (enemies == 3 && upSpace == 1 && ok) defPoint -= 200;
        ok = false;

        //xuống
        for (int count = 1; count <= 4 && row > 4 && column > 4; count++)
        {
            if (caroMatrix[row - count, column - count] == turn)
            {
                if (count == 4) defPoint -= 170;
                leftAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row - count, column - count] == 0)
                {
                    if (count == 1) ok = true;
                    downSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }

        }

        if (enemies == 3 && downSpace == 1 && ok) defPoint -= 200;
        if (rightAllies > 0 && leftAllies > 0 && (upSpace + downSpace + enemies) < 4) return 0;
        defPoint -= listAtkPoint[rightAllies + leftAllies];
        defPoint += listDefPoint[enemies];
        return defPoint;
    }

    //chéo ngược
    public int CheckDefSubDiagonal(int row, int column)
    {
        int defPoint = 0;
        int leftAllies = 0;
        int rightAllies = 0;
        int enemies = 0;
        int upSpace = 0;
        int downSpace = 0;
        bool ok = false;
        int turn = botTurn ? 2 : 1;
        //lên
        for (int count = 1; count <= 4 && row > 4 && column < width - 5; count++)
        {

            if (caroMatrix[row - count, column + count] == turn)
            {
                if (count == 4) defPoint -= 170;
                rightAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row - count, column + count] == 0)
                {
                    if (count == 1) ok = true;
                    upSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }
        }
        if (enemies == 3 && upSpace == 1 && ok) defPoint -= 200;
        ok = false;

        //xuống
        for (int count = 1; count <= 4 && row < height - 5 && column > 4; count++)
        {
            if (caroMatrix[row + count, column - count] == turn)
            {
                if (count == 4) defPoint -= 170;
                leftAllies++;
                break;
            }
            else
            {
                if (caroMatrix[row + count, column - count] == 0)
                {
                    if (count == 1) ok = true;
                    downSpace++;
                }
                else
                {
                    if (count == 1) defPoint += 9;
                    enemies++;
                }
            }
        }

        if (enemies == 3 && downSpace == 1 && ok) defPoint -= 200;
        if (rightAllies > 0 && leftAllies > 0 && (upSpace + downSpace + enemies) < 4) return 0;
        defPoint -= listAtkPoint[leftAllies + rightAllies];
        defPoint += listDefPoint[enemies];
        return defPoint;
    }

    #endregion

    #endregion

    #region duyệt chiến thắng theo 8 hướng
    //kiểm tra chiến thắng
    public void Check(int row, int column)
    {
        int turn = currentTurn ? 2 : 1;
        caroMatrix[row, column] = turn;
        //duyệt theo 8 hướng mỗi quân cờ
        if (CheckRightRow(row, column, turn) || CheckLeftRow(row, column, turn) 
            || CheckUpColumn(row, column, turn) || CheckDownColumn(row, column, turn) 
            || CheckUpMainDiagonal(row, column, turn) || CheckDownMainDiagonal(row, column, turn) 
            || CheckDownSubDiagonal(row, column, turn) || CheckUpSubDiagonal(row, column, turn))
            {
                Win();
            }
    }

    public bool CheckRightRow(int row, int column, int turn)
    {
        if (column > width - 5) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row, column + count] != turn) return false;
        }
        return true;
    }

    public bool CheckLeftRow(int row, int column, int turn)
    {
        if (column < 4) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row, column - count] != turn) return false;
        }
        return true;
    }

    public bool CheckUpColumn(int row, int column, int turn)
    {
        if (row < 4) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row - count, column] != turn) return false;
        }
        return true;
    }

    public bool CheckDownColumn(int row, int column, int turn)
    {
        if (row > height - 5) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row + count, column] != turn) return false;
        }
        return true;
    }

    public bool CheckUpMainDiagonal(int row, int column, int turn)
    {
        if (row < 4 || column < 4) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row - count, column - count] != turn) return false;
        }
        return true;
    }

    public bool CheckDownMainDiagonal(int row, int column, int turn)
    {
        if (row > height - 5 || column > width - 5) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row + count, column + count] != turn) return false;
        }
        return true;
    }

    public bool CheckDownSubDiagonal(int row, int column, int turn)
    {
        if (row > height - 5 || column < 4) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row + count, column - count] != turn) return false;
        }
        return true;
    }

    public bool CheckUpSubDiagonal(int row, int column, int turn)
    {
        if (row < 4 || column > width - 5) return false;
        for (int count = 1; count <= 4; count++)
        {
            if (caroMatrix[row - count, column + count] != turn) return false;
        }
        return true;
    }

    #endregion

}
