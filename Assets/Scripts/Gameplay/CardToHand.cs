using System.Linq;
using UnityEngine;

public class CardToHand : MonoBehaviour
{
    public bool initialized = false;

    public GameObject hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        hand = GameObject.Find("Hand");
        gameObject.transform.SetParent(hand.transform);
    }

    public void PlayCard() {
        if (!initialized) return;
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        TurnSystem turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();

        Card card = gameObject.GetComponent<DisplayCard>().displayCard;
        if (turnSystem.isPlayerTurn && (playerDeck.cardsToDraw == 0 || (playerDeck.cardsToDraw > 0 && card.num == CardNum.DRAW2 || card.num == CardNum.DRAW4))) {
            Card topDiscard = playerDeck.discardPile.Last();
            if (playerDeck.isCardPlayable(card)) {
                Debug.Log("[Player] Played card: " + card.color + " " + card.num + " on " + topDiscard.color + " " + topDiscard.num);
                playerDeck.PlayCard(gameObject);
                turnSystem.playerPlayed = true;
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

