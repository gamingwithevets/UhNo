using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerDeck : MonoBehaviour

{
    public static int deckSize;
    public List<Card> deck = new List<Card>();
    public static List<Card> staticDeck = new List<Card>();

    public GameObject cardInDeckTop;
    public GameObject cardInDeck1;
    public GameObject cardInDeck2;
    public GameObject cardInDeck3;
    public GameObject cardInDeck4;

    public GameObject cardToHand;
    public GameObject cardToHandO;
    public GameObject topd;
    public GameObject topDiscard;

    public List<GameObject> playerClones = new List<GameObject>();
    public List<GameObject> opponentClones = new List<GameObject>();
    public GameObject hand;

    public List<Card> discardPile = new List<Card>();

    public int cardsToDraw = 0;
    public bool drawed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = new List<Card>(CardDatabase.cardList);
        deckSize = deck.Count;

        StartCoroutine(StartGame());
    }

    public void Reset() {
        GameObject.Find("TurnSystem").GetComponent<TurnSystem>().Reset();
        GameObject.Find("ColorPicker").GetComponent<ColorPicker>().Reset();
        Destroy(topDiscard);
        discardPile.Clear();
        cardsToDraw = 0;
        drawed = false;
        foreach (GameObject card in playerClones) Destroy(card);
        foreach (GameObject card in opponentClones) Destroy(card);
        playerClones.Clear();
        opponentClones.Clear();

        deck = new List<Card>(CardDatabase.cardList);
        deckSize = deck.Count;
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()

    {
        if (deck.Count > deckSize) deck.RemoveRange(deckSize, deck.Count - deckSize);
        staticDeck = deck;

        hand = GameObject.Find("Hand");

        cardInDeckTop = cardInDeck4;
        if (deckSize < 30) {
            cardInDeck4.SetActive(false);
            cardInDeckTop = cardInDeck3;
        }
        if (deckSize < 20) {
            cardInDeck3.SetActive(false);
            cardInDeckTop = cardInDeck2;
        }

        if (deckSize < 5) {
            cardInDeck2.SetActive(false);
            cardInDeckTop = cardInDeck1;
        }

        if (deckSize < 1) {
            cardInDeck1.SetActive(false);
            cardInDeckTop = null;
        }

        if (discardPile.Count > 0) {
            topDiscard.SetActive(true);
            topDiscard.GetComponent<DisplayCard>().displayCard = discardPile[discardPile.Count - 1];
        }
    }

    IEnumerator StartGame() {
        Debug.Log("Starting game");

        GameObject canvas = GameObject.Find("Canvas");

        Shuffle();
        for (int i = 0; i < 7; i++) {
            yield return new WaitForSeconds(0.25f);
            playerClones.Add(Instantiate(cardToHand, transform.position, transform.rotation));
            yield return new WaitUntil(() => playerClones[i].GetComponent<CardToHand>().initialized);
            opponentClones.Add(Instantiate(cardToHandO, transform.position, transform.rotation));
            yield return new WaitUntil(() => opponentClones[i].GetComponent<CardToHandO>().initialized);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        Card topCard = deck[deckSize - 1];
        while (topCard.color == CardColor.WILD && topCard.num == CardNum.DRAW4) {
            deck.RemoveAt(deckSize - 1);
            deck.Insert(0, topCard);
            topCard = deck[deckSize - 1];
        }
        topDiscard = Instantiate(topd, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        topDiscard.transform.position = cardInDeckTop.transform.position;
        topDiscard.transform.SetParent(canvas.transform);
        yield return new WaitUntil(() => topDiscard.GetComponent<DisplayCard>().initialized);
        topDiscard.GetComponent<TopDiscardAnim>().StartAnim();

        Card card = topDiscard.GetComponent<DisplayCard>().displayCard;
        yield return new WaitForSeconds(0.5f);

        discardPile.Add(card);
        if (card.color == CardColor.WILD) OpenColorPicker();
        if (card.num == CardNum.DRAW2) cardsToDraw += 2;

        TurnSystem turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
        turnSystem.EndOpponentTurn();
    }

    public void Shuffle() {
        var rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    public void PlayCard(GameObject card) {
        Card realCard = card.GetComponent<DisplayCard>().displayCard;
        playerClones.Remove(card);
        card.GetComponent<CardToHandAnim>().StartPlayAnim();
        if (realCard.color == CardColor.WILD) OpenColorPicker();
        else GameObject.Find("TurnSystem").GetComponent<TurnSystem>().IsWildTurn = false;

    }

    public void PlayCardO(GameObject card) {
        card.GetComponent<CardToHandO>().played = true;
        card.GetComponent<CardToHandO>().transform.rotation = Quaternion.identity;
        opponentClones.Remove(card);
        card.GetComponent<CardToHandAnim>().StartPlayAnim();
    }

    public bool isCardPlayable(Card card) {
        return card.color == CardColor.WILD ||
            card.color == discardPile.Last().color ||
            card.num == discardPile.Last().num;
    }


    public void DrawCard() {
        Debug.Log("[Player] Drawing card");
        TurnSystem turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
        Card deckCard = deck[deckSize - 1];
        if (turnSystem.IsPlayerTurn && (cardsToDraw > 0 || (cardsToDraw == 0 && !drawed))) {

            playerClones.Add(Instantiate(cardToHand, transform.position, transform.rotation));
            drawed = true;
        }
        if (cardsToDraw > 0) {
            --cardsToDraw;
            if (cardsToDraw == 0) turnSystem.EndPlayerTurn();
        } else if (!isCardPlayable(deckCard)) turnSystem.EndPlayerTurn();
    }

    public void DrawCardO() {
        Debug.Log("[Opponent] Drawing card");
        opponentClones.Add(Instantiate(cardToHandO, transform.position, transform.rotation));
    }

    public void RefillDeck() {
        deck = new List<Card>(discardPile.Take(discardPile.Count - 1));
        discardPile.RemoveRange(0, discardPile.Count - 1);
        deckSize = deck.Count;
        Shuffle();
        GameObject.Find("TurnSystem").GetComponent<TurnSystem>().SetTurnWarning("DECK REFILLED!");
    }

    void OpenColorPicker() {
        ColorPicker colorPicker = GameObject.Find("ColorPicker").GetComponent<ColorPicker>();
        colorPicker.PickColor();
    }

}
