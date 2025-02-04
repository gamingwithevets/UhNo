using System.Collections;
using System.Linq;
using UnityEngine;

public class CardToHand : MonoBehaviour
{
    public bool initialized = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.position = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>().cardInDeckTop.transform.position;
        StartCoroutine(StartAnim());
        initialized = true;
    }

    void Update() {
        gameObject.GetComponent<DisplayCard>().cardBack = false;
    }

    IEnumerator StartAnim() {
        yield return new WaitUntil(() => gameObject.GetComponent<CardToHandAnim>().initialized);
        gameObject.GetComponent<CardToHandAnim>().StartCardToHandAnim();
    }

    public void PlayCard() {
        if (!initialized) return;
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        TurnSystem turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();

        Card card = gameObject.GetComponent<DisplayCard>().displayCard;
        if (turnSystem.IsPlayerTurn && (playerDeck.cardsToDraw == 0 || (playerDeck.cardsToDraw > 0 && !playerDeck.drawed && (card.num == CardNum.DRAW2 || card.num == CardNum.DRAW4)))) {
            Card topDiscard = playerDeck.discardPile.Last();
            if (playerDeck.isCardPlayable(card)) {
                Debug.Log("[Player] Played card: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                playerDeck.PlayCard(gameObject);
                turnSystem.PlayerPlayed = true;
                if (card.num == CardNum.DRAW2) playerDeck.cardsToDraw += 2;
                else if (card.num == CardNum.DRAW4) playerDeck.cardsToDraw += 4;
                turnSystem.EndPlayerTurn();
            } else {
                Debug.Log("[Player] Illegal move: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                turnSystem.SetTurnWarning("ILLEGAL MOVE!\nTRY A DIFFERENT CARD.");
            }
        }

    }
}

