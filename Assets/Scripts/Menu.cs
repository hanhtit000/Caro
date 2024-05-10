using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button[] playButton;
    [SerializeField] private Button[] exitButton;
    [SerializeField] private Sprite caroX;
    [SerializeField] private Sprite caroO;
    [SerializeField] private Sprite checkBotMode;
    private int width;
    private int height;
    private bool currentTurn;
    private bool botMode;

    public Sprite CaroX { get { return caroX; } }
    public Sprite CaroO { get { return caroO; } }

    public Sprite CheckBotMode { get { return checkBotMode; } }
    public int Width
    {
        get { return width; }
        set { width = value; }
    }

    public int Height
    {
        get { return height; }
        set { height = value; }
    }
    public bool CurrentTurn
    {
        get { return currentTurn; }
        set { currentTurn = value; }
    }

    public bool BotMode
    {
        get { return botMode; }
        set { botMode = value; }
    }
    // Start is called before the first frame update
    public virtual void Awake()
    {
        foreach(var a in playButton) 
        {
            a.onClick.AddListener(PlayGame);
        }
        foreach (var a in exitButton)
        {
            a.onClick.AddListener(Exit);
        }
        Width = StaticData.width != 0 ? StaticData.width : 10;
        Height = StaticData.height != 0 ? StaticData.height : 10;
        CurrentTurn = StaticData.currentTurn;
        BotMode = StaticData.botMode;
    }
    private void PlayGame()
    {
        StaticData.width = Width;
        StaticData.height = Height;
        StaticData.currentTurn = CurrentTurn;
        StaticData.botMode = BotMode;
        SceneManager.LoadSceneAsync("GameScene");
    }
    private void Exit()
    {
        Application.Quit();
    }
}
