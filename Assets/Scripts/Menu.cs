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

    public Sprite CaroX { get { return caroX; } }
    public Sprite CaroO { get { return caroO; } }

    public Sprite CheckBotMode { get { return checkBotMode; } }

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
        StaticData.width = StaticData.width != 0 ? StaticData.width : 10;
        StaticData.height = StaticData.height != 0 ? StaticData.height : 10;
    }
    private void PlayGame()
    {
        SceneManager.LoadSceneAsync("GameScene");
    }
    private void Exit()
    {
        Application.Quit();
    }
}
