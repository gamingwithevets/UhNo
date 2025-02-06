using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


public class TurnSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TurnText;
    [SerializeField] private TextMeshProUGUI m_WildColorText;
    [SerializeField] private GameObject m_YouWin;
    [SerializeField] private GameObject m_YouLose;

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

    public static TurnSystem GetInstance() {
        return GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_TurnText.text = "";
        m_WildColorText.text = "";
        m_TurnText.gameObject.SetActive(true);
        m_WildColorText.gameObject.SetActive(true);
        m_IsPlayerTurn = false;
        m_PlayerTurn = 0;
        m_OpponentTurn = 0;
    }

    public void Reset()
    {
        m_IsTurnWarningActive = false;
        m_YouWin.SetActive(false);
        m_YouLose.SetActive(false);
        m_IsPlayerTurn = false;
        m_PlayerTurn = 0;
        m_OpponentTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsTurnWarningActive) m_TurnText.text = m_TurnWarning;
        else
        {
            if (m_IsPlayerTurn)
            {
                if (PlayerDeck.cardsToDraw > 0) m_TurnText.text = "DRAW " + PlayerDeck.cardsToDraw + " CARD(S)";
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
                case CardColor.RED:
                    m_WildColorText.text = "COLOR: <mark=#ffff0080><color=#FF0000>RED</color></mark>";
                    break;
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
        m_IsTurnWarningActive = false;
        PlayerDeck.drawed = false;
        m_IsPlayerTurn = false;
        if (PlayerDeck.GetInstance().playerClones.Count == 0)
        {
            m_YouWin.transform.SetAsLastSibling();

            m_YouWin.SetActive(true);
            return;
        }
        ++m_OpponentTurn;
        if (m_OpponentTurn > 0) StartCoroutine(OpponentAI());
    }

    public void EndOpponentTurn()
    {
        m_IsTurnWarningActive = false;
        if (PlayerDeck.GetInstance().opponentClones.Count == 0)
        {
            m_YouLose.transform.SetAsLastSibling();
            m_YouLose.SetActive(true);
            return;
        }
        ++m_PlayerTurn;
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

    IEnumerator OpponentAI()
    {
        yield return new WaitUntil(() => !ColorPicker.GetInstance().active);

        yield return new WaitForSeconds(0.5f);
        Card topDiscard = PlayerDeck.GetInstance().discardPile.Last();
        switch (topDiscard.num)
        {
            case CardNum.SKIP:
            case CardNum.REVERSE:
                if (!m_PlayerPlayed) goto default;
                PlayerDeck.GetInstance().ReadyForNextMove = true;
                break;
            default:
                if (PlayerDeck.cardsToDraw > 0)
                {
                    GameObject cardToPlay = null;
                    List<CardColor> colors = new List<CardColor>();
                    foreach (GameObject card in PlayerDeck.GetInstance().opponentClones)
                    {
                        Card realCard = card.GetComponent<DisplayCard>().displayCard;
                        if ((realCard.num == CardNum.DRAW2 || realCard.num == CardNum.DRAW4) && PlayerDeck.GetInstance().isCardPlayable(realCard))
                        {
                            cardToPlay = card;
                            break;
                        }
                        else colors.Add(card.GetComponent<DisplayCard>().displayCard.color);
                    }
                    if (cardToPlay)
                    {
                        Debug.Log("[Opponent] Played card: " + cardToPlay.GetComponent<DisplayCard>().displayCard.color + " " + cardToPlay.GetComponent<DisplayCard>().displayCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                        if (cardToPlay.GetComponent<DisplayCard>().displayCard.color == CardColor.WILD) {
                            cardToPlay.GetComponent<DisplayCard>().displayCard.color = colors.Count > 0 ? colors.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First() : (CardColor)Random.Range(0, 4);
                        }
                        yield return PlayerDeck.GetInstance().PlayCardO(cardToPlay);
                        topDiscard = PlayerDeck.GetInstance().discardPile.Last();
                        if (topDiscard.num == CardNum.DRAW2) PlayerDeck.cardsToDraw += 2;
                        else if (topDiscard.num == CardNum.DRAW4) PlayerDeck.cardsToDraw += 4;

                    }
                    else
                    {
                        while (PlayerDeck.cardsToDraw > 0)
                        {
                            PlayerDeck.cardsToDraw -= 1;
                            PlayerDeck.GetInstance().DrawCardO();
                            yield return new WaitForSeconds(0.25f);
                        }
                        PlayerDeck.GetInstance().ReadyForNextMove = true;
                    }
                    break;
                }
                yield return new WaitForSeconds(0.5f);
                bool played = false;
                while (!played)
                {
                    GameObject wild = null;
                    GameObject cardToPlay = null;
                    List<CardColor> colors = new List<CardColor>();
                    foreach (GameObject card in PlayerDeck.GetInstance().opponentClones)
                    {
                        if (card.GetComponent<DisplayCard>().displayCard.color == CardColor.WILD) wild = card;
                        else if (card.GetComponent<DisplayCard>().displayCard.color == topDiscard.color)
                        {
                            cardToPlay = card;
                            break;
                        }
                        else if (card.GetComponent<DisplayCard>().displayCard.num == topDiscard.num)
                        {
                            cardToPlay = card;
                            break;
                        }
                        else colors.Add(card.GetComponent<DisplayCard>().displayCard.color);
                    }
                    if (cardToPlay)
                    {
                        Debug.Log("[Opponent] Played card: " + cardToPlay.GetComponent<DisplayCard>().displayCard.color + " " + cardToPlay.GetComponent<DisplayCard>().displayCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                        if (cardToPlay.GetComponent<DisplayCard>().displayCard.num == CardNum.SKIP || cardToPlay.GetComponent<DisplayCard>().displayCard.num == CardNum.REVERSE) m_Skip = true;
                        if (cardToPlay.GetComponent<DisplayCard>().displayCard.num == CardNum.DRAW2) PlayerDeck.cardsToDraw += 2;
                        yield return PlayerDeck.GetInstance().PlayCardO(cardToPlay);
                        played = true;
                        m_IsWildTurn = false;
                    }
                    else if (wild)
                    {
                        Debug.Log("[Opponent] Played wild card: " + wild.GetComponent<DisplayCard>().displayCard.color + " " + wild.GetComponent<DisplayCard>().displayCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                        wild.GetComponent<DisplayCard>().displayCard.color = colors.Count > 0 ? colors.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First() : (CardColor)Random.Range(0, 4);
                        if (wild.GetComponent<DisplayCard>().displayCard.num == CardNum.DRAW4) PlayerDeck.cardsToDraw += 4;
                        yield return PlayerDeck.GetInstance().PlayCardO(wild);
                        played = true;
                    }
                    else
                    {
                        PlayerDeck.GetInstance().DrawCardO();
                        Card drawnCard = PlayerDeck.GetInstance().opponentClones.Last().GetComponent<DisplayCard>().displayCard;
                        if (PlayerDeck.GetInstance().isCardPlayable(drawnCard))
                        {
                            yield return new WaitForSeconds(0.5f);
                            Debug.Log("[Opponent] Played card: " + drawnCard.color + " " + drawnCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                            yield return PlayerDeck.GetInstance().PlayCardO(PlayerDeck.GetInstance().opponentClones.Last());
                        } else PlayerDeck.GetInstance().ReadyForNextMove = true;
                        played = true;
                    }
                }
                break;
        }
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
