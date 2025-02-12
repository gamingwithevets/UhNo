using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardToHand : MonoBehaviour
{
    public bool initialized = false;

    void Start()
    {
        transform.SetParent(FindFirstObjectByType<GameplayView>().transform);
        transform.position = PlayerDeck.Instance.cardInDeckTop.transform.position;
        StartCoroutine(StartAnim());
        initialized = true;
    }

    void Update()
    {
        DisplayCard displayCard = gameObject.GetComponent<DisplayCard>();
        displayCard.IsCardBack = !gameObject.GetComponent<CardToHandAnim>().m_Animated;
        if (PlayerDeck.Instance.discardPile.Count > 0)
        {
            displayCard.Image.color = PlayerDeck.Instance.isCardPlayable(displayCard.CardInfo) ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
            gameObject.GetComponent<Button>().interactable = PlayerDeck.Instance.isCardPlayable(displayCard.CardInfo);
        }
    }

    IEnumerator StartAnim()
    {
        yield return new WaitUntil(() => gameObject.GetComponent<CardToHandAnim>().m_Initialized);
        gameObject.GetComponent<CardToHandAnim>().StartCardToHandAnim();
    }

    IEnumerator PlayCardCoroutine()
    {
        if (!initialized || !PlayerDeck.Instance.ReadyForNextMove) yield break;
        Card card = gameObject.GetComponent<DisplayCard>().CardInfo;
        if (TurnSystem.Instance.IsPlayerTurn && (PlayerDeck.cardsToDraw == 0 || (PlayerDeck.cardsToDraw > 0 && !PlayerDeck.drawed && (card.num == CardNum.DRAW2 || card.num == CardNum.DRAW4))))
        {
            Card topDiscard = PlayerDeck.Instance.discardPile.Last();
            if (PlayerDeck.Instance.isCardPlayable(card))
            {
                Debug.Log("[Player] Played card: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                int cardsToDraw = 0;
                if (card.num == CardNum.DRAW2) cardsToDraw = 2;
                else if (card.num == CardNum.DRAW4) cardsToDraw = 4;
                yield return PlayerDeck.Instance.PlayCard(gameObject, cardsToDraw);
                TurnSystem.Instance.PlayerPlayed = true;
                PlayerDeck.cardsToDraw += cardsToDraw;
                TurnSystem.Instance.EndPlayerTurn();
            }
            else
            {
                Debug.Log("[Player] Illegal move: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                TurnSystem.Instance.SetTurnWarning("ILLEGAL MOVE!");
            }
        }
    }

    public void PlayCard()
    {
        StartCoroutine(PlayCardCoroutine());
    }
}

