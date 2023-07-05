using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    [SerializeField] private Menu menu;

    private void Start()
    {
        GameMaster.instance.StartNewGame();
    }

    public void GameEnd(bool isWin)
    {
        menu.OpenPanel(isWin);
    }

    public void RestartGame()
    {
        GameMaster.instance.StartNewGame();
    }
}
