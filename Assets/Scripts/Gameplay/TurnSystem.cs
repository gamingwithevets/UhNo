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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_IsPlayerTurn = false;
        m_PlayerTurn = 0;
        m_OpponentTurn = 0;
    }

    public void Reset()
    {
        m_YouWin.SetActive(false);
        m_YouLose.SetActive(false);
        m_IsPlayerTurn = false;
        m_PlayerTurn = 0;
        m_OpponentTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        if (m_IsTurnWarningActive) m_TurnText.text = m_TurnWarning;
        else
        {
            if (m_IsPlayerTurn)
            {
                if (playerDeck.cardsToDraw > 0) m_TurnText.text = "DRAW " + playerDeck.cardsToDraw + " CARD(S)";
                else m_TurnText.text = "PLAY A CARD!";
            }
            else
            {
                if (playerDeck.cardsToDraw > 0) m_TurnText.text = "OPPONENT DRAWS " + playerDeck.cardsToDraw + " CARD(S)";
                else m_TurnText.text = m_OpponentTurn > 0 ? "OPPONENT PLAYS" : "PLEASE WAIT, SETTING UP...";
            }
        }


        if (m_IsWildTurn)
        {
            switch (m_WildColor)
            {
                case CardColor.RED:
                    m_WildColorText.text = "COLOR: <color=#FF0000>RED</color>";
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
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        playerDeck.drawed = false;
        m_IsPlayerTurn = false;
        if (playerDeck.playerClones.Count == 0)
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
        if (GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>().opponentClones.Count == 0)
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
        ColorPicker colorPicker = GameObject.Find("ColorPicker").GetComponent<ColorPicker>();
        yield return new WaitUntil(() => !colorPicker.active);

        yield return new WaitForSeconds(0.5f);
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        Card topDiscard = playerDeck.discardPile.Last();
        switch (topDiscard.num)
        {
            case CardNum.SKIP:
                if (!m_PlayerPlayed) goto default;
                break;
            default:
                if (playerDeck.cardsToDraw > 0)
                {
                    GameObject cardToPlay = null;
                    List<CardColor> colors = new List<CardColor>();
                    foreach (GameObject card in playerDeck.opponentClones)
                    {
                        Card realCard = card.GetComponent<DisplayCard>().displayCard;
                        if ((realCard.num == CardNum.DRAW2 || realCard.num == CardNum.DRAW4) && playerDeck.isCardPlayable(realCard))
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
                        playerDeck.PlayCardO(cardToPlay);
                        topDiscard = playerDeck.discardPile.Last();
                        if (topDiscard.num == CardNum.DRAW2) playerDeck.cardsToDraw += 2;
                        else if (topDiscard.num == CardNum.DRAW4) playerDeck.cardsToDraw += 4;
                    }
                    else
                    {
                        while (playerDeck.cardsToDraw > 0)
                        {
                            playerDeck.cardsToDraw -= 1;
                            playerDeck.DrawCardO();
                            yield return new WaitForSeconds(0.25f);
                        }
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
                    foreach (GameObject card in playerDeck.opponentClones)
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
                        playerDeck.PlayCardO(cardToPlay);
                        played = true;
                        m_IsWildTurn = false;
                        if (cardToPlay.GetComponent<DisplayCard>().displayCard.num == CardNum.SKIP) m_Skip = true;
                        if (cardToPlay.GetComponent<DisplayCard>().displayCard.num == CardNum.DRAW2) playerDeck.cardsToDraw += 2;
                    }
                    else if (wild)
                    {
                        Debug.Log("[Opponent] Played wild card: " + wild.GetComponent<DisplayCard>().displayCard.color + " " + wild.GetComponent<DisplayCard>().displayCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                        wild.GetComponent<DisplayCard>().displayCard.color = colors.Count > 0 ? colors.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First() : (CardColor)Random.Range(0, 4);
                        playerDeck.PlayCardO(wild);
                        played = true;
                        if (wild.GetComponent<DisplayCard>().displayCard.num == CardNum.DRAW4) playerDeck.cardsToDraw += 4;
                    }
                    else
                    {
                        playerDeck.DrawCardO();
                        Card drawnCard = playerDeck.opponentClones.Last().GetComponent<DisplayCard>().displayCard;
                        if (playerDeck.isCardPlayable(drawnCard))
                        {
                            yield return new WaitForSeconds(0.5f);
                            Debug.Log("[Opponent] Played card: " + drawnCard.color + " " + drawnCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                            playerDeck.PlayCardO(playerDeck.opponentClones.Last());
                        }
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
