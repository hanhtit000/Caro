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
            if (currentTurn == botTurn)
            {
                BotCheck();
            }
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

        //thuật toán minmax tìm điểm cao nhất để đánh
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //nếu nước cờ chưa có ai đánh và không bị cắt tỉa thì mới xét giá trị MinMax
                if (caroMatrix[i, j] == 0 && !catTia(i, j))
                {
                    int DiemTam;

                    atkPoint = CheckAtkRow(i, j) + CheckAtkColumn(i, j) + CheckAtkMainDiagonal(i, j) + CheckAtkSubDiagonal(i, j);
                    defPoint = CheckDefRow(i, j) + duyetPN_Doc(i, j) + duyetPN_CheoXuoi(i, j) + duyetPN_CheoNguoc(i, j);

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
                        oco = new C_OCo(MangOCo[i, j].Dong, MangOCo[i, j].Cot, MangOCo[i, j].SoHuu);
                    }
                }
            }
        }
        danhCo(g, oco.Cot * C_OCo.CHIEU_RONG + 1, oco.Dong * C_OCo.CHIEU_CAO + 1);
    }


    #region Cắt tỉa Alpha betal
    bool catTia(int row, int column)
    {
        //nếu cả 4 hướng đều không có nước cờ thì cắt tỉa
        if (catTiaNgang(row, column) && catTiaDoc(row, column) && catTiaCheoPhai(row, column) && catTiaCheoTrai(row, column))
            return true;

        //chạy đến đây thì 1 trong 4 hướng vẫn có nước cờ thì không được cắt tỉa
        return false;
    }

    bool catTiaNgang(int row, int column)
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
    bool catTiaDoc(int row, int column)
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
    bool catTiaCheoPhai(int row, int column)
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
    bool catTiaCheoTrai(int row, int column)
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

    private int[] listAtkPoint = new int[7] { 0, 4, 25, 246, 7300, 6561, 59049 };
    private int[] listDefPoint = new int[7] { 0, 3, 24, 243, 2197, 19773, 177957 };

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
                if (caroMatrix[row, column + count] == 0) space++;
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
                if (caroMatrix[row - count, column] == 0) space++;
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
                if (caroMatrix[row + count, column + count] == 0) space++;
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
                if (caroMatrix[row - count, column + count] == 0) space++;
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
    public int CheckDefRow(int dongHT, int cotHT)
    {
        int DiemPhongNgu = 0;
        int SoQuanTaTrai = 0;
        int SoQuanTaPhai = 0;
        int SoQuanDich = 0;
        int KhoangChongPhai = 0;
        int KhoangChongTrai = 0;
        bool ok = false;


        for (int dem = 1; dem <= 4 && cotHT < BanCo.SoCot - 5; dem++)
        {
            if (MangOCo[dongHT, cotHT + dem].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;
            }
            else
                if (MangOCo[dongHT, cotHT + dem].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaTrai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongPhai++;
            }
        }

        if (SoQuanDich == 3 && KhoangChongPhai == 1 && ok)
            DiemPhongNgu -= 200;

        ok = false;

        for (int dem = 1; dem <= 4 && cotHT > 4; dem++)
        {
            if (MangOCo[dongHT, cotHT - dem].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;
            }
            else
                if (MangOCo[dongHT, cotHT - dem].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaPhai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongTrai++;
            }
        }

        if (SoQuanDich == 3 && KhoangChongTrai == 1 && ok)
            DiemPhongNgu -= 200;

        if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTrai + KhoangChongPhai + SoQuanDich) < 4)
            return 0;

        DiemPhongNgu -= listAtkPoint[SoQuanTaPhai + SoQuanTaPhai];
        DiemPhongNgu += listDefPoint[SoQuanDich];

        return DiemPhongNgu;
    }

    //duyệt dọc
    public int duyetPN_Doc(int dongHT, int cotHT)
    {
        int DiemPhongNgu = 0;
        int SoQuanTaTrai = 0;
        int SoQuanTaPhai = 0;
        int SoQuanDich = 0;
        int KhoangChongTren = 0;
        int KhoangChongDuoi = 0;
        bool ok = false;

        //lên
        for (int dem = 1; dem <= 4 && dongHT > 4; dem++)
        {
            if (MangOCo[dongHT - dem, cotHT].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;

            }
            else
                if (MangOCo[dongHT - dem, cotHT].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaPhai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongTren++;
            }
        }

        if (SoQuanDich == 3 && KhoangChongTren == 1 && ok)
            DiemPhongNgu -= 200;

        ok = false;
        //xuống
        for (int dem = 1; dem <= 4 && dongHT < BanCo.SoDong - 5; dem++)
        {
            //gặp quân địch
            if (MangOCo[dongHT + dem, cotHT].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;
            }
            else
                if (MangOCo[dongHT + dem, cotHT].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaTrai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongDuoi++;
            }
        }

        if (SoQuanDich == 3 && KhoangChongDuoi == 1 && ok)
            DiemPhongNgu -= 200;

        if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTren + KhoangChongDuoi + SoQuanDich) < 4)
            return 0;

        DiemPhongNgu -= listAtkPoint[SoQuanTaTrai + SoQuanTaPhai];
        DiemPhongNgu += listDefPoint[SoQuanDich];
        return DiemPhongNgu;
    }

    //chéo xuôi
    public int duyetPN_CheoXuoi(int dongHT, int cotHT)
    {
        int DiemPhongNgu = 0;
        int SoQuanTaTrai = 0;
        int SoQuanTaPhai = 0;
        int SoQuanDich = 0;
        int KhoangChongTren = 0;
        int KhoangChongDuoi = 0;
        bool ok = false;

        //lên
        for (int dem = 1; dem <= 4 && dongHT < BanCo.SoDong - 5 && cotHT < BanCo.SoCot - 5; dem++)
        {
            if (MangOCo[dongHT + dem, cotHT + dem].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;
            }
            else
                if (MangOCo[dongHT + dem, cotHT + dem].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaPhai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongTren++;
            }
        }

        if (SoQuanDich == 3 && KhoangChongTren == 1 && ok)
            DiemPhongNgu -= 200;

        ok = false;
        //xuống
        for (int dem = 1; dem <= 4 && dongHT > 4 && cotHT > 4; dem++)
        {
            if (MangOCo[dongHT - dem, cotHT - dem].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;
            }
            else
                if (MangOCo[dongHT - dem, cotHT - dem].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaTrai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongDuoi++;
            }
        }

        if (SoQuanDich == 3 && KhoangChongDuoi == 1 && ok)
            DiemPhongNgu -= 200;

        if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTren + KhoangChongDuoi + SoQuanDich) < 4)
            return 0;

        DiemPhongNgu -= listAtkPoint[SoQuanTaPhai + SoQuanTaTrai];
        DiemPhongNgu += listDefPoint[SoQuanDich];

        return DiemPhongNgu;
    }

    //chéo ngược
    public int duyetPN_CheoNguoc(int dongHT, int cotHT)
    {
        int DiemPhongNgu = 0;
        int SoQuanTaTrai = 0;
        int SoQuanTaPhai = 0;
        int SoQuanDich = 0;
        int KhoangChongTren = 0;
        int KhoangChongDuoi = 0;
        bool ok = false;

        //lên
        for (int dem = 1; dem <= 4 && dongHT > 4 && cotHT < BanCo.SoCot - 5; dem++)
        {

            if (MangOCo[dongHT - dem, cotHT + dem].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;
            }
            else
                if (MangOCo[dongHT - dem, cotHT + dem].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaPhai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongTren++;
            }
        }


        if (SoQuanDich == 3 && KhoangChongTren == 1 && ok)
            DiemPhongNgu -= 200;

        ok = false;

        //xuống
        for (int dem = 1; dem <= 4 && dongHT < BanCo.SoDong - 5 && cotHT > 4; dem++)
        {
            if (MangOCo[dongHT + dem, cotHT - dem].SoHuu == 2)
            {
                if (dem == 1)
                    DiemPhongNgu += 9;

                SoQuanDich++;
            }
            else
                if (MangOCo[dongHT + dem, cotHT - dem].SoHuu == 1)
            {
                if (dem == 4)
                    DiemPhongNgu -= 170;

                SoQuanTaTrai++;
                break;
            }
            else
            {
                if (dem == 1)
                    ok = true;

                KhoangChongDuoi++;
            }
        }

        if (SoQuanDich == 3 && KhoangChongDuoi == 1 && ok)
            DiemPhongNgu -= 200;

        if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTren + KhoangChongDuoi + SoQuanDich) < 4)
            return 0;

        DiemPhongNgu -= listAtkPoint[SoQuanTaTrai + SoQuanTaPhai];
        DiemPhongNgu += listDefPoint[SoQuanDich];

        return DiemPhongNgu;
    }

    #endregion

    #endregion

    #region duyệt chiến thắng theo 8 hướng
    //kiểm tra chiến thắng
    public bool kiemTraChienThang(Graphics g)
    {
        if (_stkCacNuocDaDi.Count != 0)
        {
            foreach (C_OCo oco in _stkCacNuocDaDi)
            {
                //duyệt theo 8 hướng mỗi quân cờ
                if (duyetNgangPhai(g, oco.Dong, oco.Cot, oco.SoHuu) || duyetNgangTrai(g, oco.Dong, oco.Cot, oco.SoHuu)
                    || duyetDocTren(g, oco.Dong, oco.Cot, oco.SoHuu) || duyetDocDuoi(g, oco.Dong, oco.Cot, oco.SoHuu)
                    || duyetCheoXuoiTren(g, oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoXuoiDuoi(g, oco.Dong, oco.Cot, oco.SoHuu)
                    || duyetCheoNguocTren(g, oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoNguocDuoi(g, oco.Dong, oco.Cot, oco.SoHuu))
                {
                    ketThucTroChoi(oco);
                    return true;
                }
            }
        }

        return false;
    }

    //vẽ đường kẻ trên 5 nước thắng
    public void veDuongChienThang(Graphics g, int x1, int y1, int x2, int y2)
    {
        g.DrawLine(new Pen(Color.Blue, 3f), x1, y1, x2, y2);
    }

    public bool duyetNgangPhai(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (cotHT > BanCo.SoCot - 5)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT, cotHT + dem].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, (cotHT) * C_OCo.CHIEU_RONG, dongHT * C_OCo.CHIEU_CAO + C_OCo.CHIEU_CAO / 2, (cotHT + 5) * C_OCo.CHIEU_RONG, dongHT * C_OCo.CHIEU_CAO + C_OCo.CHIEU_CAO / 2);
        return true;
    }

    public bool duyetNgangTrai(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (cotHT < 4)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT, cotHT - dem].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, (cotHT + 1) * C_OCo.CHIEU_RONG, dongHT * C_OCo.CHIEU_CAO + C_OCo.CHIEU_CAO / 2, (cotHT - 4) * C_OCo.CHIEU_RONG, dongHT * C_OCo.CHIEU_CAO + C_OCo.CHIEU_CAO / 2);
        return true;
    }

    public bool duyetDocTren(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (dongHT < 4)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT - dem, cotHT].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, cotHT * C_OCo.CHIEU_RONG + C_OCo.CHIEU_RONG / 2, (dongHT + 1) * C_OCo.CHIEU_CAO, cotHT * C_OCo.CHIEU_RONG + C_OCo.CHIEU_RONG / 2, (dongHT - 4) * C_OCo.CHIEU_CAO);
        return true;
    }

    public bool duyetDocDuoi(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (dongHT > BanCo.SoDong - 5)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT + dem, cotHT].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, cotHT * C_OCo.CHIEU_RONG + C_OCo.CHIEU_RONG / 2, dongHT * C_OCo.CHIEU_CAO, cotHT * C_OCo.CHIEU_RONG + C_OCo.CHIEU_RONG / 2, (dongHT + 5) * C_OCo.CHIEU_CAO);
        return true;
    }

    public bool duyetCheoXuoiTren(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (dongHT < 4 || cotHT < 4)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT - dem, cotHT - dem].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, (cotHT + 1) * C_OCo.CHIEU_RONG, (dongHT + 1) * C_OCo.CHIEU_CAO, (cotHT - 4) * C_OCo.CHIEU_RONG, (dongHT - 4) * C_OCo.CHIEU_CAO);
        return true;
    }

    public bool duyetCheoXuoiDuoi(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (dongHT > BanCo.SoDong - 5 || cotHT > BanCo.SoCot - 5)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT + dem, cotHT + dem].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, cotHT * C_OCo.CHIEU_RONG, dongHT * C_OCo.CHIEU_CAO, (cotHT + 5) * C_OCo.CHIEU_RONG, (dongHT + 5) * C_OCo.CHIEU_CAO);
        return true;
    }

    public bool duyetCheoNguocDuoi(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (dongHT > BanCo.SoDong - 5 || cotHT < 4)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT + dem, cotHT - dem].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, (cotHT + 1) * C_OCo.CHIEU_RONG, dongHT * C_OCo.CHIEU_CAO, (cotHT - 4) * C_OCo.CHIEU_RONG, (dongHT + 5) * C_OCo.CHIEU_CAO);
        return true;
    }

    public bool duyetCheoNguocTren(Graphics g, int dongHT, int cotHT, int SoHuu)
    {
        if (dongHT < 4 || cotHT > BanCo.SoCot - 5)
            return false;
        for (int dem = 1; dem <= 4; dem++)
        {
            if (MangOCo[dongHT - dem, cotHT + dem].SoHuu != SoHuu)
            {
                return false;
            }

        }
        veDuongChienThang(g, cotHT * C_OCo.CHIEU_RONG, (dongHT + 1) * C_OCo.CHIEU_CAO, (cotHT + 5) * C_OCo.CHIEU_RONG, (dongHT - 4) * C_OCo.CHIEU_CAO);
        return true;
    }

    #endregion

}
