using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
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

    [SerializeField] private CardColor m_NextColor;
    [SerializeField] private CardNum m_NextNum;
    public static int cardsToDraw = 0;
    public static bool drawed = false;

    [SerializeField] private bool m_ReadyForNextMove = true;

    public CardColor NextColor
    {
        get => m_NextColor;
        set => m_NextColor = value;
    }
    public CardNum NextNum
    {
        get => m_NextNum;
        set => m_NextNum = value;
    }

    public bool ReadyForNextMove

    {
        get => m_ReadyForNextMove;
        set => m_ReadyForNextMove = value;
    }

    public static PlayerDeck GetInstance()
    {
        return GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = new List<Card>(CardDatabase.cardList);
        deckSize = deck.Count;

        StartCoroutine(StartGame());
    }

    public void Reset()
    {
        Destroy(topDiscard);
        discardPile.Clear();
        cardsToDraw = 0;
        drawed = false;
        m_ReadyForNextMove = true;
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
        if (deckSize < 30)
        {
            cardInDeck4.SetActive(false);
            cardInDeckTop = cardInDeck3;
        }
        if (deckSize < 20)
        {
            cardInDeck3.SetActive(false);
            cardInDeckTop = cardInDeck2;
        }

        if (deckSize < 5)
        {
            cardInDeck2.SetActive(false);
            cardInDeckTop = cardInDeck1;
        }

        if (deckSize < 1)
        {
            cardInDeck1.SetActive(false);
            cardInDeckTop = null;
        }

        if (discardPile.Count > 0)
        {
            topDiscard.SetActive(true);
            topDiscard.GetComponent<DisplayCard>().CardInfo = discardPile[discardPile.Count - 1];
        }
    }

    IEnumerator StartGame()
    {
        Debug.Log("Starting game");

        Transform gameplayView = FindFirstObjectByType<GameplayView>().transform;

        Shuffle();
        for (int i = 0; i < 7; i++)
        {
            yield return new WaitForSeconds(0.25f);
            playerClones.Add(Instantiate(cardToHand, transform.position, transform.rotation));
            yield return new WaitUntil(() => playerClones[i].GetComponent<CardToHand>().initialized);
            opponentClones.Add(Instantiate(cardToHandO, transform.position, transform.rotation));
            yield return new WaitUntil(() => opponentClones[i].GetComponent<CardToHandO>().initialized);
        }

        yield return new WaitForSeconds(0.5f);

        Card topCard = deck[deckSize - 1];
        while (topCard.color == CardColor.WILD && topCard.num == CardNum.DRAW4)
        {
            deck.RemoveAt(deckSize - 1);
            deck.Insert(0, topCard);
            topCard = deck[deckSize - 1];
        }
        topDiscard = Instantiate(topd, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        topDiscard.transform.position = cardInDeckTop.transform.position;
        topDiscard.transform.SetParent(gameplayView);
        yield return new WaitUntil(() => topDiscard.GetComponent<DisplayCard>().Initialized);
        topDiscard.GetComponent<TopDiscardAnim>().StartAnim();

        Card card = topDiscard.GetComponent<DisplayCard>().CardInfo;
        yield return new WaitForSeconds(0.5f);

        discardPile.Add(card);
        if (card.num == CardNum.DRAW2) cardsToDraw += 2;
        if (card.num == CardNum.SKIP || card.num == CardNum.REVERSE) TurnSystem.GetInstance().Skip = true;

        NextColor = card.color;
        NextNum = card.num;

        TurnSystem.GetInstance().EndOpponentTurn();

    }

    public void Shuffle()
    {
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

    public IEnumerator PlayCard(GameObject card, int cardsToDraw)
    {
        m_ReadyForNextMove = false;
        Card realCard = card.GetComponent<DisplayCard>().CardInfo;
        playerClones.Remove(card);
        Debug.Log("[Player] Waiting for animation");
        card.GetComponent<CardToHandAnim>().StartPlayAnim();
        yield return new WaitUntil(() => card.GetComponent<CardToHandAnim>().m_Destroyed);
        Debug.Log("[Player] Animation finished");
        NextColor = realCard.color;
        NextNum = realCard.num;
        if (realCard.color == CardColor.WILD && playerClones.Count > 0) yield return OpenColorPicker(cardsToDraw);
        else TurnSystem.GetInstance().IsWildTurn = false;
    }

    public IEnumerator PlayCardO(GameObject card)
    {
        card.GetComponent<CardToHandO>().played = true;
        card.GetComponent<CardToHandO>().transform.rotation = Quaternion.identity;
        opponentClones.Remove(card);
        Debug.Log("[Opponent] Waiting for animation");
        card.GetComponent<CardToHandAnim>().StartPlayAnim();
        yield return new WaitUntil(() => card.GetComponent<CardToHandAnim>().m_Destroyed);
        Debug.Log("[Opponent] Animation finished");
        Card realCard = card.GetComponent<DisplayCard>().CardInfo;
        NextColor = realCard.color;
        NextNum = realCard.num;
        m_ReadyForNextMove = true;
    }

    public bool isCardPlayable(Card card)
    {
        if (!TurnSystem.GetInstance().IsPlayerTurn) return false;
        if (cardsToDraw > 0) return (card.num == CardNum.DRAW2 && (card.color == NextColor || NextNum == CardNum.DRAW2)) || card.num == CardNum.DRAW4;
        return NextColor == CardColor.WILD ||
            card.color == CardColor.WILD ||

            card.color == NextColor ||
            card.num == NextNum;
    }

    public void DrawCard()
    {
        StartCoroutine(DrawCardCoroutine());
    }

    public IEnumerator DrawCardCoroutine()
    {
        m_ReadyForNextMove = false;
        Debug.Log("[Player] Drawing card");
        Card deckCard = deck[deckSize - 1];
        if (TurnSystem.GetInstance().IsPlayerTurn && (cardsToDraw > 0 || (cardsToDraw == 0 && !drawed)))
        {
            GameObject card = Instantiate(cardToHand, transform.position, transform.rotation);
            yield return new WaitUntil(() => card.GetComponent<CardToHandAnim>().m_Animated);
            playerClones.Add(card);
            drawed = true;
        }
        if (cardsToDraw > 0)
        {
            --cardsToDraw;
            if (cardsToDraw == 0) TurnSystem.GetInstance().EndPlayerTurn();
            else m_ReadyForNextMove = true;
        }
        else
        {
            if (!isCardPlayable(deckCard)) TurnSystem.GetInstance().EndPlayerTurn();
            else m_ReadyForNextMove = true;
        }
    }

    public IEnumerator DrawCardO()
    {
        Debug.Log("[Opponent] Drawing card");
        GameObject card = Instantiate(cardToHandO, transform.position, transform.rotation);
        yield return new WaitUntil(() => card.GetComponent<CardToHandAnim>().m_Animated);
        opponentClones.Add(card);
    }

    public void RefillDeck()
    {
        deck = new List<Card>(discardPile.Take(discardPile.Count - 1));
        discardPile.RemoveRange(0, discardPile.Count - 1);
        deckSize = deck.Count;
        Shuffle();
        GameObject.Find("TurnSystem").GetComponent<TurnSystem>().SetTurnWarning("DECK REFILLED!");
    }

    IEnumerator OpenColorPicker(int cardsToDraw = 0)
    {
        ColorPicker colorPicker = ColorPicker.GetInstance();
        colorPicker.PickColor(cardsToDraw);
        yield return null;
    }
}
