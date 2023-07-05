using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameplayManager gameManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject winTxt;
    [SerializeField] private GameObject loseTxt;
    [SerializeField] private Button restartBtn;

    private void OnEnable()
    {
        restartBtn.onClick.AddListener(() =>
        {
            ClosePanel();
            //gameManager.RestartGame();
        });
    }

    private void OnDisable()
    {
        restartBtn.onClick.RemoveAllListeners();
    }

    public void OpenPanel(bool isWin)
    {
        panel.SetActive(true);
        winTxt.SetActive(isWin);
        loseTxt.SetActive(!isWin);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);

        Preloader.Instance.LoadNewScene("Menu");
    }
}
