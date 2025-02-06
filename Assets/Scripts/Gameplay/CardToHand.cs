using System.Collections;
using System.Linq;
using UnityEngine;

public class CardToHand : MonoBehaviour
{
    public bool initialized = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.SetParent(GameObject.FindFirstObjectByType<GameplayView>().transform);
        transform.position = PlayerDeck.GetInstance().cardInDeckTop.transform.position;
        StartCoroutine(StartAnim());
        initialized = true;
    }

    void Update() {
        gameObject.GetComponent<DisplayCard>().cardBack = !gameObject.GetComponent<CardToHandAnim>().m_Animated;
    }

    IEnumerator StartAnim() {
        yield return new WaitUntil(() => gameObject.GetComponent<CardToHandAnim>().m_Initialized);
        gameObject.GetComponent<CardToHandAnim>().StartCardToHandAnim();
    }

    IEnumerator PlayCardCoroutine()
    {
        if (!initialized || !PlayerDeck.GetInstance().ReadyForNextMove) yield break;
        Card card = gameObject.GetComponent<DisplayCard>().displayCard;
        if (TurnSystem.GetInstance().IsPlayerTurn && (PlayerDeck.cardsToDraw == 0 || (PlayerDeck.cardsToDraw > 0 && !PlayerDeck.drawed && (card.num == CardNum.DRAW2 || card.num == CardNum.DRAW4))))
        {
            Card topDiscard = PlayerDeck.GetInstance().discardPile.Last();
            if (PlayerDeck.GetInstance().isCardPlayable(card))
            {
                Debug.Log("[Player] Played card: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                if (card.num == CardNum.DRAW2) PlayerDeck.cardsToDraw += 2;
                else if (card.num == CardNum.DRAW4) PlayerDeck.cardsToDraw += 4;
                yield return PlayerDeck.GetInstance().PlayCard(gameObject);
                TurnSystem.GetInstance().PlayerPlayed = true;
                TurnSystem.GetInstance().EndPlayerTurn();
            }
            else
            {
                Debug.Log("[Player] Illegal move: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                TurnSystem.GetInstance().SetTurnWarning("ILLEGAL MOVE!");
            }
        }
    }

    public void PlayCard() {
        StartCoroutine(PlayCardCoroutine());
    }
}

