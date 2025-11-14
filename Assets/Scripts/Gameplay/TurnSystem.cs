using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI m_TurnText;
    [SerializeField] private TextMeshProUGUI m_WildColorText;

    [Header("Read Only")]
    [SerializeField] private bool m_IsPlayerTurn;
    [SerializeField] private bool m_PlayerPlayed = false;
    [SerializeField] private bool m_Skip = false;
    [SerializeField] private bool m_IsTurnWarningActive = false;
    [SerializeField] private bool m_IsWildTurn;
    [SerializeField] private int m_PlayerTurn;
    [SerializeField] private int m_OpponentTurn;
    [SerializeField] private string m_TurnWarning;
    [SerializeField] private CardColor m_WildColor;

    public bool IsPlayerTurn
    {
        get => m_IsPlayerTurn;
        set => m_IsPlayerTurn = value;
    }

    public bool Skip
    {
        get => m_Skip;
        set => m_Skip = value;
    }

    public bool IsWildTurn

    {
        get => m_IsWildTurn;
        set => m_IsWildTurn = value;
    }

    public bool PlayerPlayed
    {
        get => m_PlayerPlayed;
        set => m_PlayerPlayed = value;
    }

    void Awake() {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_TurnText.text = "";
        m_WildColorText.text = "";
        m_TurnText.gameObject.SetActive(true);
        m_WildColorText.gameObject.SetActive(true);
        m_IsPlayerTurn = false;
        m_IsWildTurn = false;
        m_PlayerTurn = 0;
        m_OpponentTurn = 0;
    }

    public void Reset() => Start();

    // Update is called once per frame
    void Update()
    {
        if (m_IsTurnWarningActive) m_TurnText.text = m_TurnWarning;
        else
        {
            if (m_IsPlayerTurn)
            {
                if (PlayerDeck.cardsToDraw > 0) m_TurnText.text = "DRAW " + PlayerDeck.cardsToDraw + " CARD(S)";
                else if (PlayerDeck.uhNoActive) m_TurnText.text = "YOU HAVE ONE CARD LEFT!";
                else m_TurnText.text = "PLAY A CARD!";
            }
            else
            {
                if (PlayerDeck.cardsToDraw > 0) m_TurnText.text = "OPPONENT DRAWS " + PlayerDeck.cardsToDraw + " CARD(S)";
                else m_TurnText.text = m_OpponentTurn > 0 ? "OPPONENT PLAYS" : "PLEASE WAIT, SETTING UP...";
            }
        }


        if (m_IsWildTurn)
        {
            switch (m_WildColor)
            {
                case CardColor.BLUE:
                    m_WildColorText.text = "COLOR: <color=#007EFF>BLUE</color>";
                    break;
                case CardColor.GREEN:
                    m_WildColorText.text = "COLOR: <color=#28BC00>GREEN</color>";
                    break;
                case CardColor.YELLOW:
                    m_WildColorText.text = "COLOR: <color=#FFCC00>YELLOW</color>";
                    break;
                default:
                    m_WildColorText.text = "COLOR: " + m_WildColor.ToString();
                    break;
            }
        }
        else m_WildColorText.text = "";
    }

    public void EndPlayerTurn()
    {
        PlayerDeck.cardsToDraw += PlayerDeck.cardsToDrawAdd;
        PlayerDeck.cardsToDrawAdd = 0;
        m_IsTurnWarningActive = false;
        PlayerDeck.drawed = false;
        m_IsPlayerTurn = false;
        if (PlayerDeck.Instance.playerClones.Count == 0)
        {
            m_TurnText.gameObject.SetActive(false);
            ViewManager.Instance.ShowView(ViewId.WinView, false);
            return;
        }

        ++m_OpponentTurn;
        if (m_OpponentTurn > 0) StartCoroutine(OpponentAI());
    }

    public void EndOpponentTurn()
    {
        m_IsTurnWarningActive = false;
        if (PlayerDeck.Instance.opponentClones.Count == 0)
        {
            Lose();
            return;
        }
        ++m_PlayerTurn;
        PlayerDeck.uhNoActive = false;
        ButtonUhNo.Instance.Update();
        if (m_Skip)
        {
            m_PlayerPlayed = false;
            m_Skip = false;
            ++m_OpponentTurn;
            if (m_OpponentTurn > 0) StartCoroutine(OpponentAI());
        }
        else
        {
            m_IsPlayerTurn = true;
            m_PlayerPlayed = false;
        }
    }

    public void Lose()
    {
        m_TurnText.gameObject.SetActive(false);
        m_WildColorText.gameObject.SetActive(false);
        ViewManager.Instance.ShowView(ViewId.LoseView, false);
    }

    IEnumerator OpponentAI()
    {
        int cardsToDraw = 0;

        Card topDiscard = PlayerDeck.Instance.discardPile.Last();
        switch (topDiscard.num)
        {
            case CardNum.SKIP:
            case CardNum.REVERSE:
                if (!m_PlayerPlayed) goto default;
                Debug.Log("[Opponent] Skip/Reverse detected!");
                PlayerDeck.Instance.ReadyForNextMove = true;
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                if (PlayerDeck.cardsToDraw > 0)
                {
                    GameObject cardToPlay = null;
                    List<CardColor> colors = new List<CardColor>();
                    foreach (GameObject card in PlayerDeck.Instance.opponentClones)
                    {
                        Card realCard = card.GetComponent<DisplayCard>().CardInfo;
                        if (realCard.num == CardNum.DRAW2 || realCard.num == CardNum.DRAW4)
                        {
                            cardToPlay = card;
                            break;
                        }
                        else colors.Add(card.GetComponent<DisplayCard>().CardInfo.color);
                    }
                    if (cardToPlay)
                    {
                        if (cardToPlay.GetComponent<DisplayCard>().CardInfo.color == CardColor.WILD)
                        {
                            Debug.Log("[Opponent] Played wild card: " + cardToPlay.GetComponent<DisplayCard>().CardInfo.color + " " + cardToPlay.GetComponent<DisplayCard>().CardInfo.num + " on " + topDiscard.color + " " + topDiscard.num);
                            CardColor color = colors.Count > 0 ? colors.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First() : (CardColor)Random.Range(0, 4);
                            yield return PlayerDeck.Instance.PlayCardO(cardToPlay);
                            if (GameManager.Instance.GetHouseRule("WildSample") && cardToPlay.GetComponent<DisplayCard>().CardInfo.num == CardNum.COLOR) m_Skip = true;
                            else
                            {

                                PlayerDeck.Instance.NextColor = color;
                                SetWildTurn(color);
                                Debug.Log("[Opponent] Set next color to " + color);
                            }
                        }
                        else
                        {
                            Debug.Log("[Opponent] Played card: " + cardToPlay.GetComponent<DisplayCard>().CardInfo.color + " " + cardToPlay.GetComponent<DisplayCard>().CardInfo.num + " on " + topDiscard.color + " " + topDiscard.num);
                            yield return PlayerDeck.Instance.PlayCardO(cardToPlay);
                        }
                        topDiscard = PlayerDeck.Instance.discardPile.Last();
                        if (topDiscard.num == CardNum.DRAW2) cardsToDraw = 2;
                        else if (topDiscard.num == CardNum.DRAW4) cardsToDraw = 4;

                    }
                    else
                    {
                        while (PlayerDeck.cardsToDraw > 0)
                        {
                            PlayerDeck.cardsToDraw -= 1;
                            yield return PlayerDeck.Instance.DrawCardO();
                        }
                        PlayerDeck.Instance.ReadyForNextMove = true;
                    }
                    break;
                }
                yield return new WaitForSeconds(0.5f);
                bool played = false;
                while (!played)
                {
                    yield return null;
                    GameObject wild = null;
                    GameObject cardToPlay = null;
                    List<CardColor> colors = new List<CardColor>();
                    foreach (GameObject card in PlayerDeck.Instance.opponentClones)
                    {
                        if (card.GetComponent<DisplayCard>().CardInfo.color == CardColor.WILD) {
                            wild = card;
                            break;
                        }
                        else if ((topDiscard.color == CardColor.WILD && GameManager.Instance.GetHouseRule("WildSample")) || card.GetComponent<DisplayCard>().CardInfo.color == PlayerDeck.Instance.NextColor)
                        {
                            cardToPlay = card;
                            break;
                        }
                        else if (card.GetComponent<DisplayCard>().CardInfo.num == topDiscard.num)
                        {
                            cardToPlay = card;
                            break;
                        }
                        else colors.Add(card.GetComponent<DisplayCard>().CardInfo.color);
                    }
                    if (cardToPlay)
                    {
                        Debug.Log("[Opponent] Played card: " + cardToPlay.GetComponent<DisplayCard>().CardInfo.color + " " + cardToPlay.GetComponent<DisplayCard>().CardInfo.num + " on " + topDiscard.color + " " + topDiscard.num);
                        if (cardToPlay.GetComponent<DisplayCard>().CardInfo.num == CardNum.SKIP || cardToPlay.GetComponent<DisplayCard>().CardInfo.num == CardNum.REVERSE) m_Skip = true;
                        if (cardToPlay.GetComponent<DisplayCard>().CardInfo.num == CardNum.DRAW2) cardsToDraw = 2;
                        yield return PlayerDeck.Instance.PlayCardO(cardToPlay);
                        played = true;
                        m_IsWildTurn = false;
                    }
                    else if (wild)
                    {
                        Debug.Log("[Opponent] Played wild card: " + wild.GetComponent<DisplayCard>().CardInfo.color + " " + wild.GetComponent<DisplayCard>().CardInfo.num + " on " + topDiscard.color + " " + topDiscard.num);
                        CardColor color = colors.Count > 0 ? colors.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First() : (CardColor)Random.Range(0, 4);
                        if (wild.GetComponent<DisplayCard>().CardInfo.num == CardNum.DRAW4) cardsToDraw = 4;
                        yield return PlayerDeck.Instance.PlayCardO(wild);
                        if (GameManager.Instance.GetHouseRule("WildSample") && wild.GetComponent<DisplayCard>().CardInfo.num == CardNum.COLOR) m_Skip = true;
                        else
                        {
                            PlayerDeck.Instance.NextColor = color;
                            SetWildTurn(color);
                            Debug.Log("[Opponent] Set next color to " + color);
                        }
                        played = true;
                    }
                    else
                    {
                        yield return PlayerDeck.Instance.DrawCardO();
                        Card drawnCard = PlayerDeck.Instance.opponentClones.Last().GetComponent<DisplayCard>().CardInfo;
                        if (PlayerDeck.Instance.isCardPlayableO(drawnCard))
                        {
                            if (drawnCard.color == CardColor.WILD)
                            {
                                Debug.Log("[Opponent] Played wild card: " + drawnCard.color + " " + drawnCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                                CardColor color = colors.Count > 0 ? colors.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First() : (CardColor)Random.Range(0, 4);
                                yield return PlayerDeck.Instance.PlayCardO(PlayerDeck.Instance.opponentClones.Last());
                                if (drawnCard.num == CardNum.DRAW4) cardsToDraw = 4;
                                if (GameManager.Instance.GetHouseRule("WildSample") && drawnCard.num == CardNum.COLOR) m_Skip = true;
                                else
                                {
                                    PlayerDeck.Instance.NextColor = color;
                                    SetWildTurn(color);
                                    Debug.Log("[Opponent] Set next color to " + color);
                                }
                            }
                            else
                            {
                                Debug.Log("[Opponent] Played card: " + drawnCard.color + " " + drawnCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                                if (drawnCard.num == CardNum.SKIP || drawnCard.num == CardNum.REVERSE) m_Skip = true;
                                if (drawnCard.num == CardNum.DRAW2) cardsToDraw = 2;
                                yield return PlayerDeck.Instance.PlayCardO(PlayerDeck.Instance.opponentClones.Last());
                            }
                        }
                        else PlayerDeck.Instance.ReadyForNextMove = true;
                        played = true;
                    }
                }
                break;
        }
        PlayerDeck.cardsToDraw += cardsToDraw;
        EndOpponentTurn();
    }

    public void SetTurnWarning(string warningText)
    {
        m_IsTurnWarningActive = true;
        m_TurnWarning = warningText;
        StartCoroutine(TurnWarningTimer());
    }

    IEnumerator TurnWarningTimer()
    {
        yield return new WaitForSeconds(5f);
        m_IsTurnWarningActive = false;
    }

    public void SetWildTurn(CardColor color)
    {
        Debug.Log("Wild Color: " + color);
        m_WildColor = color;
        m_IsWildTurn = true;
    }
}
