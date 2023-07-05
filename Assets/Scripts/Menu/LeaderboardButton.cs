using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardButton : MonoBehaviour {
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI leaderboardNameTxt;
    [SerializeField] private TextMeshProUGUI playerPlaceTxt;
    [SerializeField] private Image image;
    [SerializeField] private Sprite coloredSprite, blankSprite;
    [SerializeField] private Transform stars;

    private void OnEnable() {
        button.onClick.AddListener(ShowLeaderboardDetails);
    }

    private void OnDisable() {
        button.onClick.RemoveAllListeners();
    }

    public void Init() {
        //pass info about this leaderboard like name and place
        leaderboardNameTxt.text = "Leaderboard1";
        playerPlaceTxt.text = "1";
        int startRnd = Random.Range(0, 4);
        if (startRnd > 0) {
            image.sprite = coloredSprite;
            SetStars(startRnd);
        }
        else {
            image.sprite = blankSprite;
        }
    }

    private void ShowLeaderboardDetails() {
        //pass info for display
        LeaderboardDetails.Instance.DisplayInfo();
        LeaderboardDetails.Instance.Open();
    }

    public void SetStars(int starsCount) {
        for (int i = 0; i < starsCount; i++) {
            stars.GetChild(i).gameObject.SetActive(true);
        }
    }
}
