using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InviteFriendListElement : MonoBehaviour
{
    public Image Avatar;
    public TextMeshProUGUI Text;
    public GameObject gamePanel;
    public GameObject tournamentPanel;
    public GameObject rankedPanel;
    [SerializeField] private Button gameBtn;
    [SerializeField] private Button tournamentBtn;
    [SerializeField] private Button rankedBtn;
    [SerializeField] private Button closeBtn;

    private void OnEnable() {
        gameBtn.onClick.AddListener(ConfirmGame);
        tournamentBtn.onClick.AddListener(ConfirmTournament);
        rankedBtn.onClick.AddListener(ConfirmRanked);
        closeBtn.onClick.AddListener(Close);
    }

    private void OnDisable() {
        gameBtn.onClick.RemoveAllListeners();
        tournamentBtn.onClick.RemoveAllListeners();
        rankedBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    private void ConfirmGame()
    {
        Debug.Log("Confirm");
    }

    private void ConfirmTournament()
    {
        Debug.Log("ConfirmTournament");
    }

    private void ConfirmRanked()
    {
        Debug.Log("ConfirmRanked");
    }

    private void Close()
    {
        Debug.Log("Close");
    }
}
