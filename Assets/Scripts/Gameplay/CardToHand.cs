using System.Collections;
using System.Linq;
using UnityEngine;

public class CardToHand : MonoBehaviour
{
    public bool initialized = false;

    void Start()
    {
        transform.SetParent(FindFirstObjectByType<GameplayView>().transform);
        transform.position = PlayerDeck.GetInstance().cardInDeckTop.transform.position;
        StartCoroutine(StartAnim());
        initialized = true;
    }

    void Update()
    {
        gameObject.GetComponent<DisplayCard>().IsCardBack = !gameObject.GetComponent<CardToHandAnim>().m_Animated;
    }

    IEnumerator StartAnim()
    {
        yield return new WaitUntil(() => gameObject.GetComponent<CardToHandAnim>().m_Initialized);
        gameObject.GetComponent<CardToHandAnim>().StartCardToHandAnim();
    }

    IEnumerator PlayCardCoroutine()
    {
        if (!initialized || !PlayerDeck.GetInstance().ReadyForNextMove) yield break;
        Card card = gameObject.GetComponent<DisplayCard>().CardInfo;
        if (TurnSystem.GetInstance().IsPlayerTurn && (PlayerDeck.cardsToDraw == 0 || (PlayerDeck.cardsToDraw > 0 && !PlayerDeck.drawed && (card.num == CardNum.DRAW2 || card.num == CardNum.DRAW4))))
        {
            Card topDiscard = PlayerDeck.GetInstance().discardPile.Last();
            if (PlayerDeck.GetInstance().isCardPlayable(card))
            {
                Debug.Log("[Player] Played card: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                int cardsToDraw = 0;
                if (card.num == CardNum.DRAW2) cardsToDraw = 2;
                else if (card.num == CardNum.DRAW4) cardsToDraw = 4;
                yield return PlayerDeck.GetInstance().PlayCard(gameObject, cardsToDraw);
                TurnSystem.GetInstance().PlayerPlayed = true;
                PlayerDeck.cardsToDraw += cardsToDraw;
                TurnSystem.GetInstance().EndPlayerTurn();
            }
            else
            {
                Debug.Log("[Player] Illegal move: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                TurnSystem.GetInstance().SetTurnWarning("ILLEGAL MOVE!");
            }
        }
    }

    public void PlayCard()
    {
        StartCoroutine(PlayCardCoroutine());
    }
}

