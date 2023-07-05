using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class ViewManager : MonoBehaviour
{
    public static ViewManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);

        // Set values to variables
        zeroVector = Vector3.zero;
        halfVector = new Vector2(.5f, .5f);
        oneVector = Vector3.one;

        playgroundCards = new List<GameObject>();
        playgroundCards.Add(null);
        playgroundCards.Add(null);
        playgroundCards.Add(null);
        playgroundCards.Add(null);

        leftEyeCards = new List<GameObject>();
        rightEyeCards = new List<GameObject>();
        leftEyeDealCards = new List<GameObject>();
        rightEyeDealCards = new List<GameObject>();
    }

    [SerializeField] private List<CardContainer> cardContainers;
    public List<CardContainer> CardContainers { get { return cardContainers; } private set { } }
    [SerializeField] private Transform deck;
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    private List<GameObject> leftEyeCards;
    private List<GameObject> rightEyeCards;
    [SerializeField] private List<Transform> cardplaces;
    public List<Transform> CardPlaces { get { return cardplaces; } private set { } }
    private List<GameObject> playgroundCards;
    [SerializeField] private Text scoreTxt;

    [SerializeField] private List<TextMeshProUGUI> playerNames;
    [SerializeField] private TextMeshProUGUI playerRank;

    [SerializeField] private GameObject trumpInfo;
    [SerializeField] private Image trumpImg;
    [SerializeField] private TextMeshProUGUI pwfName;
    [SerializeField] private List<Sprite> suitSprites;

    [SerializeField] private float distributionSpeed = .2f;
    [SerializeField] private float cardFlyTime = .3f;
    [SerializeField] private float endDealTime = 2f;

    [SerializeField] private List<EyePosRot> eyePosRots;

    [SerializeField] private Image [] turnTimeCircleGreen;
    [SerializeField] private Image[] turnTimeCircleYellow;
    [SerializeField] private Image [] turnTimeCircleRed;

    [SerializeField] private GameObject endDealPanel;
    [SerializeField] private GameObject resumeGamePanel;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private GameObject [] botAvatars;

    [SerializeField] private Transform leftEyeDeal;
    [SerializeField] private Transform rightEyeDeal;
    private List<GameObject> leftEyeDealCards;
    private List<GameObject> rightEyeDealCards;
    [SerializeField] private TextMeshProUGUI playerWinScoreTxt;
    [SerializeField] private TextMeshProUGUI playerLoseScoreTxt;


    private int playerTurnId = -1;
    private DateTime turnDeadlineDT;

    private Vector3 zeroVector;
    private Vector2 halfVector;
    private Vector3 oneVector;

    private bool eyesAreReady;

    private float turnTime = 40f;
    private float turnTimeSmall = 10f;
    private int currentTimerType;


    private void OnEnable()
    {
        resumeBtn.onClick.AddListener(ResumeGame);
    }
    private void OnDisable()
    {
        resumeBtn.onClick.RemoveAllListeners();
    }
    public void ClearViews()
    {
        eyesAreReady = false;

        for (int i = 0; i < leftEyeCards.Count; i++)
        {
            Destroy(leftEyeCards[i]);
        }
        for (int i = 0; i < rightEyeCards.Count; i++)
        {
            Destroy(rightEyeCards[i]);
        }

        leftEyeCards = new List<GameObject>();
        rightEyeCards = new List<GameObject>();

        HideTrumpInfo();
    }

    public void SetPlayerNames(PlayerInfo[] playerInfos)
    {
        for (int i = 0; i < playerNames.Count; i++)
        {
            if (playerNames[i] != null)
                playerNames[i].text = playerInfos[i].name;
        }
    }

    public void SetPlayerRank(int rank)
    {
        playerRank.text = rank.ToString();
    }

    #region Distribution
    public void DistributeCards(int[] cards, Suit trump, bool online = false)
    {
        StartCoroutine(DistributeCor(cards, trump, online));
    }
    private IEnumerator DistributeCor(int[] cards, Suit trump, bool online = false)
    {
        deck.gameObject.SetActive(true);

        int[] playerCards = Card.SortCards(trump, cards);
        int playerCardIndex = 0;

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < GameResources.instance.GetCards().Length - 4; i++)
        {
            int playerIndex = i % 8 / 2;
            if (playerIndex == 0)
            {
                GiveCardPlayer(playerCards[playerCardIndex]);
                playerCardIndex++;
            }
            else
            {
                GiveCardOther(playerIndex);
            }
            yield return new WaitForSeconds(distributionSpeed);
        }

        if (!eyesAreReady)
        {
            GiveEye(true);
            GiveEyeDeal(true);
            yield return new WaitForSeconds(distributionSpeed);
            GiveEye(true);
            GiveEyeDeal(true);
            yield return new WaitForSeconds(distributionSpeed);

            GiveEye(false);
            GiveEyeDeal(false);
            yield return new WaitForSeconds(distributionSpeed);
            GiveEye(false);
            GiveEyeDeal(false);

            eyesAreReady = true;
        }

        deck.gameObject.SetActive(false);
        if (online)
            GameMasterOnline.instance.CardDistributed();
        else
            GameMaster.instance.CardDistributed();
    }
    private void GiveCardPlayer(int cardId)
    {
        GameObject newCard = Instantiate(GameResources.instance.GetCardPrefab(cardId), deck.position
            , Quaternion.identity, cardContainers[0].transform);
        DragAndDropCard dadCard = newCard.GetComponent<DragAndDropCard>();
        dadCard.SetId(cardId);
        newCard.transform.DOLocalMove(cardContainers[0].GetCardLocalPosition(), cardFlyTime).OnComplete(() =>
        {
            newCard.GetComponent<DragAndDropCard>().SetIsDragable(true);
        });
        newCard.transform.DOLocalRotateQuaternion(cardContainers[0].GetCardLocalRotation()
            , cardFlyTime);

        cardContainers[0].AddCardGO(newCard);
    }
    private void GiveCardOther(int playerIndex)
    {
        GameObject newCard = Instantiate(GameResources.instance.GetEmptyPrefab(), deck.position
            , Quaternion.identity, cardContainers[playerIndex].transform);
        newCard.transform.DOLocalMove(cardContainers[playerIndex]
            .GetCardLocalPosition(), cardFlyTime);
        newCard.transform.DOLocalRotateQuaternion(cardContainers[playerIndex]
            .GetCardLocalRotation(), cardFlyTime);

        cardContainers[playerIndex].AddCardGO(newCard);
    }
    private void GiveEye(bool isRight)
    {
        Transform destination = isRight ? rightEye : leftEye;
        GameObject newCard = Instantiate(GameResources.instance.GetEmptyBigPrefab(), deck.position, Quaternion.identity, destination);
        newCard.transform.DOLocalMove(zeroVector, cardFlyTime);
        if (isRight) rightEyeCards.Add(newCard);
        else leftEyeCards.Add(newCard);
    }
    private void GiveEyeDeal(bool isRight)
    {
        Transform destination = isRight ? rightEyeDeal : leftEyeDeal;
        GameObject newCard = Instantiate(GameResources.instance.GetEmptyBigPrefab(), deck.position, Quaternion.identity, destination);
        newCard.transform.DOLocalMove(zeroVector, cardFlyTime);
        if (isRight) rightEyeDealCards.Add(newCard);
        else leftEyeDealCards.Add(newCard);
    }
    public void DistributeReconnected(int[][] cards, Suit trump)
    {
        
        int[] playerCards = Card.SortCards(trump, cards[0]);
        for (int playerCardIndex = 0; playerCardIndex < cards[0].Length; playerCardIndex++)
        {
            GameObject newCard = Instantiate(GameResources.instance.GetCardPrefab(playerCards[playerCardIndex]), cardContainers[0].GetCardLocalPosition()
            , Quaternion.identity, cardContainers[0].transform);
            DragAndDropCard dadCard = newCard.GetComponent<DragAndDropCard>();
            dadCard.SetId(playerCards[playerCardIndex]);
            newCard.GetComponent<DragAndDropCard>().SetIsDragable(true);
            cardContainers[0].AddCardGO(newCard);
        }
        for (int i = 1; i < 4; i++)
        {
            foreach (int j in cards[i])
            {
                GameObject newCard = Instantiate(GameResources.instance.GetEmptyPrefab(), cardContainers[i].GetCardLocalPosition()
                , Quaternion.identity, cardContainers[i].transform);
                cardContainers[i].AddCardGO(newCard);
            }
        }

        if (!eyesAreReady)
        {
            GiveEye(true);
            GiveEye(true);
            
            GiveEye(false);
            GiveEye(false);
            GiveEyeDeal(true);
            GiveEyeDeal(true);

            GiveEyeDeal(false);
            GiveEyeDeal(false);
            eyesAreReady = true;
        }

        GameMasterOnline.instance.CardDistributed();
        
    }

    #endregion


    public void SetCurrentTurnTimeCircleId(int id, int type)
    {
        playerTurnId = id;
        turnDeadlineDT = DateTime.Now.AddSeconds(type == 1 ? turnTime : turnTimeSmall);
        currentTimerType = type;
    }
    public void StopTurnTime(int id)
    {
        turnTimeCircleGreen[id].gameObject.SetActive(false);
        turnTimeCircleYellow[id].gameObject.SetActive(false);
        turnTimeCircleRed[id].gameObject.SetActive(false);
        playerTurnId = playerTurnId == id ? -1 : playerTurnId;
    }
    private void Update()
    {
        if (playerTurnId > -1)
        {
            float secondsLeft = (float)(turnDeadlineDT - DateTime.Now).TotalSeconds;
            if (secondsLeft < turnTime / 3f)
            {
                turnTimeCircleGreen[playerTurnId].gameObject.SetActive(false);
                turnTimeCircleYellow[playerTurnId].gameObject.SetActive(false);
                turnTimeCircleRed[playerTurnId].gameObject.SetActive(true);
                turnTimeCircleRed[playerTurnId].fillAmount = secondsLeft / (currentTimerType == 1 ? turnTime : turnTimeSmall);
            }
            else if (secondsLeft < turnTime / 3f * 2)
            {
                turnTimeCircleYellow[playerTurnId].gameObject.SetActive(true);
                turnTimeCircleRed[playerTurnId].gameObject.SetActive(false);
                turnTimeCircleGreen[playerTurnId].gameObject.SetActive(false);
                turnTimeCircleYellow[playerTurnId].fillAmount = secondsLeft / (currentTimerType == 1 ? turnTime : turnTimeSmall);
            }
            else
            {
                turnTimeCircleYellow[playerTurnId].gameObject.SetActive(false);
                turnTimeCircleRed[playerTurnId].gameObject.SetActive(false);
                turnTimeCircleGreen[playerTurnId].gameObject.SetActive(true);
                turnTimeCircleGreen[playerTurnId].fillAmount = secondsLeft / (currentTimerType == 1 ? turnTime : turnTimeSmall);
            }
            if (playerTurnId == 0 && secondsLeft < 0)
            {
                if (currentTimerType == 1)
                    SetCurrentTurnTimeCircleId(playerTurnId, 2);
                else
                    GameMasterOnline.instance.SetReadyToPlaceCards(false);
            }
            else if (secondsLeft < 0)
            {
                if (currentTimerType == 1)
                    SetCurrentTurnTimeCircleId(playerTurnId, 2);
            }
        }
    }

    #region MakeTurn
    public void MakeTurnPlayer(int id)
    {
        GameObject cardGO = cardContainers[0].GetCardGO(id);
        cardGO.transform.SetParent(cardplaces[0]);
        cardContainers[0].RelocateCards();
        cardGO.transform.DOLocalMove(zeroVector, cardFlyTime);
        playgroundCards[0] = cardGO;
    }
    public void MakeTurnOther(int card, int playerIndex)
    {
        if(playerIndex == 0)
            cardContainers[playerIndex].RemovePlayerCardGO(card);
        else
            cardContainers[playerIndex].RemoveCardGO();
        cardContainers[playerIndex].RelocateCards();
        GameObject cardGO = Instantiate(GameResources.instance.GetCardPrefab(card), cardContainers[playerIndex].transform.position
            , Quaternion.identity, cardplaces[playerIndex]);
        RectTransform cardRT = cardGO.GetComponent<RectTransform>();
        cardRT.pivot = halfVector;
        cardGO.transform.DOLocalMove(zeroVector, cardFlyTime);
        playgroundCards[playerIndex] = cardGO;
    }
    public void MakeTurnPlayerReconnected(int id)
    {
        GameObject cardGO = cardContainers[0].GetCardGO(id);
        cardGO.transform.SetParent(cardplaces[0]);
        cardGO.transform.localPosition = zeroVector;
        playgroundCards[0] = cardGO;
    }
    public void MakeTurnOtherReconnected(int card, int playerIndex)
    {
        GameObject cardGO = Instantiate(GameResources.instance.GetCardPrefab(card), cardContainers[playerIndex].transform.position
            , Quaternion.identity, cardplaces[playerIndex]);
        RectTransform cardRT = cardGO.GetComponent<RectTransform>();
        cardRT.pivot = halfVector;
        cardGO.transform.localPosition = zeroVector;
        playgroundCards[playerIndex] = cardGO;
    }
    #endregion

    #region RoundEnd
    public void GiveWinnerCards(int playerIndex, int firstPoints, int secondPoints)
    {
        StartCoroutine(GiveWinnerCardsCor(playerIndex, firstPoints, secondPoints));
    }
    private IEnumerator GiveWinnerCardsCor(int playerIndex, int firstPoints, int secondPoints)
    {
        for (int i = 0; i < playgroundCards.Count; i++)
        {
            playgroundCards[i].transform.SetParent(cardContainers[playerIndex].transform);
            playgroundCards[i].transform.DOLocalMove(zeroVector, cardFlyTime);
        }

        yield return new WaitForSeconds(cardFlyTime);
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < playgroundCards.Count; i++)
        {
            Destroy(playgroundCards[i]);
        }

        RefreshTeamsPoints(firstPoints, secondPoints);
    }
    public void RefreshTeamsPoints(int firstTeamPoints, int secondTeamPoints)
    {
        scoreTxt.text = firstTeamPoints + ":" + secondTeamPoints;
    }
    #endregion

    #region Suit
    public void OpenSuit(Suit suit, int playerIndex)
    {
        trumpInfo.SetActive(true);
        trumpImg.sprite = suitSprites[(int)suit];
        if(!GameManager.instance.online)
            pwfName.text = GameMaster.instance.GetPlayerInfos()[playerIndex].name;
        else
            pwfName.text = GameMasterOnline.instance.GetPlayerInfos()[playerIndex].name;
    }
    private void HideTrumpInfo()
    {
        trumpInfo.SetActive(false);
    }
    #endregion

    #region Eyes
    public void SetUpEyes(Suit[] suits)
    {
        Destroy(leftEyeCards[0]);
        Destroy(leftEyeCards[1]);
        Destroy(rightEyeCards[0]);
        Destroy(rightEyeCards[1]);

        GameObject leftBotCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[2]]));
        RectTransform leftBotCardRT = leftBotCard.GetComponent<RectTransform>();
        leftBotCardRT.pivot = halfVector;
        leftBotCard.transform.SetParent(leftEye);
        leftBotCard.transform.localPosition = zeroVector;
        leftBotCard.transform.localScale = oneVector;
        leftEyeCards[0] = leftBotCard;

        GameObject leftTopCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[0]]));
        RectTransform leftTopCardRT = leftTopCard.GetComponent<RectTransform>();
        leftTopCardRT.pivot = halfVector;
        leftTopCard.transform.SetParent(leftEye);
        leftTopCard.transform.localPosition = zeroVector;
        leftTopCard.transform.localScale = oneVector;
        leftTopCard.GetComponent<DragAndDropCard>().SetBGActive(true);
        leftEyeCards[1] = leftTopCard;

        GameObject rightBotCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[3]]));
        RectTransform rightBotCarddRT = rightBotCard.GetComponent<RectTransform>();
        rightBotCarddRT.pivot = halfVector;
        rightBotCard.transform.SetParent(rightEye);
        rightBotCard.transform.localPosition = zeroVector;
        rightBotCard.transform.localScale = oneVector;
        rightEyeCards[0] = rightBotCard;

        GameObject rightTopCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[1]]));
        RectTransform rightTopCardRT = rightTopCard.GetComponent<RectTransform>();
        rightTopCardRT.pivot = halfVector;
        rightTopCard.transform.SetParent(rightEye);
        rightTopCard.transform.localPosition = zeroVector;
        rightTopCard.transform.localScale = oneVector;
        rightTopCard.GetComponent<DragAndDropCard>().SetBGActive(true);
        rightEyeCards[1] = rightTopCard;
    }

    public void OpenEyes(bool isRight, int count)
    {
        StartCoroutine(OpenEyesCor(isRight, count));
    }

    private IEnumerator OpenEyesCor(bool isRight, int count)
    {
        EyePosRot eyePosRot = eyePosRots[count - 1];
        Transform topCardTransform = isRight ? rightEyeCards[1].transform : leftEyeCards[1].transform;

        topCardTransform.DOLocalMove(eyePosRot.TopCardPosition, cardFlyTime);
        topCardTransform.DOLocalRotate(eyePosRot.TopCardRotation, cardFlyTime);

        yield return new WaitForSeconds(cardFlyTime);

        topCardTransform.gameObject.SetActive(eyePosRot.TopCardActive);
        topCardTransform.GetComponent<DragAndDropCard>().SetBGActive(!eyePosRot.TopCardOpened);
    }
    #endregion

    public void BotPlaying(int id)
    {
        botAvatars[id].SetActive(true);
        if (id == 0)
            resumeGamePanel.SetActive(true);
        else
            playerNames[id].text = "Бот";
    }
    public void PlayerPlaying(int id, string name)
    {
        if (id == 0 && resumeGamePanel.activeSelf)
            resumeGamePanel.SetActive(false);
        else
            playerNames[id].text = name;
        if (botAvatars[id].activeSelf)
            botAvatars[id].SetActive(false);        
    }
    private void ResumeGame()
    {
        ServiceIO.Instance.ResumeGame();
    }

    public void SetUpDealEyes(Suit[] suits)
    {
        Destroy(leftEyeDealCards[0]);
        Destroy(leftEyeDealCards[1]);
        Destroy(rightEyeDealCards[0]);
        Destroy(rightEyeDealCards[1]);

        GameObject leftBotCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[2]]));
        RectTransform leftBotCardRT = leftBotCard.GetComponent<RectTransform>();
        leftBotCardRT.pivot = halfVector;
        leftBotCard.transform.SetParent(leftEyeDeal);
        leftBotCard.transform.localPosition = zeroVector;
        leftBotCard.transform.localScale = oneVector;
        leftEyeDealCards[0] = leftBotCard;

        GameObject leftTopCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[0]]));
        RectTransform leftTopCardRT = leftTopCard.GetComponent<RectTransform>();
        leftTopCardRT.pivot = halfVector;
        leftTopCard.transform.SetParent(leftEyeDeal);
        leftTopCard.transform.localPosition = zeroVector;
        leftTopCard.transform.localScale = oneVector;
        leftTopCard.GetComponent<DragAndDropCard>().SetBGActive(true);
        leftEyeDealCards[1] = leftTopCard;

        GameObject rightBotCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[3]]));
        RectTransform rightBotCarddRT = rightBotCard.GetComponent<RectTransform>();
        rightBotCarddRT.pivot = halfVector;
        rightBotCard.transform.SetParent(rightEyeDeal);
        rightBotCard.transform.localPosition = zeroVector;
        rightBotCard.transform.localScale = oneVector;
        rightEyeDealCards[0] = rightBotCard;

        GameObject rightTopCard = Instantiate(GameResources.instance.GetCardPrefab(Card.GetSixes()[(int)suits[1]]));
        RectTransform rightTopCardRT = rightTopCard.GetComponent<RectTransform>();
        rightTopCardRT.pivot = halfVector;
        rightTopCard.transform.SetParent(rightEyeDeal);
        rightTopCard.transform.localPosition = zeroVector;
        rightTopCard.transform.localScale = oneVector;
        rightTopCard.GetComponent<DragAndDropCard>().SetBGActive(true);
        rightEyeDealCards[1] = rightTopCard;
    }
    public void SetScore(bool win, int points1, int points2)
    {
        if (win)
        {
            leftEyeDeal.parent.gameObject.SetActive(true);
            rightEyeDeal.parent.gameObject.SetActive(false);
            playerWinScoreTxt.text = points1 + " : " + points2;
        }
        else
        {
            leftEyeDeal.parent.gameObject.SetActive(false);
            rightEyeDeal.parent.gameObject.SetActive(true);
            playerLoseScoreTxt.text = points2 + " : " + points1;
        }
    }
    public void OpenEyesDeal(bool isRight, int count)
    {
        StartCoroutine(OpenEyesDealCor(isRight, count));
    }

    private IEnumerator OpenEyesDealCor(bool isRight, int count)
    {
        endDealPanel.SetActive(true);
        EyePosRot eyePosRot = eyePosRots[count - 1];
        Transform topCardTransform = isRight ? rightEyeDealCards[1].transform : leftEyeDealCards[1].transform;

        topCardTransform.DOLocalMove(eyePosRot.TopCardPosition, cardFlyTime);
        topCardTransform.DOLocalRotate(eyePosRot.TopCardRotation, cardFlyTime);

        yield return new WaitForSeconds(cardFlyTime);

        topCardTransform.gameObject.SetActive(eyePosRot.TopCardActive);
        topCardTransform.GetComponent<DragAndDropCard>().SetBGActive(!eyePosRot.TopCardOpened);
        yield return new WaitForSeconds(endDealTime);
        endDealPanel.SetActive(false);
    }
}

[System.Serializable]
public class EyePosRot
{
    [SerializeField] private bool topCardActive;
    public bool TopCardActive { get { return topCardActive; } private set { } }
    [SerializeField] private bool topCardOpened;
    public bool TopCardOpened { get { return topCardOpened; } private set { } }
    [SerializeField] private Vector3 topCardPosition;
    public Vector3 TopCardPosition { get { return topCardPosition; } private set { } }
    [SerializeField] private Vector3 topCardRotation;
    public Vector3 TopCardRotation { get { return topCardRotation; } private set { } }
}
