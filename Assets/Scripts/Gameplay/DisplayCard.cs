using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour
{
    public bool initialized = false;
 
    public Card displayCard;

    public CardNum num;
    public CardColor color;
    public Sprite spriteImage;

    public Image image;

    public bool cardBack;

    public GameObject hand;
    public int numberOfCardsInDeck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numberOfCardsInDeck = PlayerDeck.deckSize;
        initialized = true;
    }


    // Update is called once per frame
    void Update()
    {
        num = displayCard.num;
        color = displayCard.color;
        spriteImage = displayCard.spriteImage;

        image.sprite = spriteImage == null ? Resources.Load<Sprite>("Cards/back") : spriteImage;

        if (this.tag == "Clone") {
            displayCard = PlayerDeck.staticDeck[numberOfCardsInDeck - 1];
            --numberOfCardsInDeck;
            --PlayerDeck.deckSize;
            if (PlayerDeck.deckSize == 0) GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>().RefillDeck();
            cardBack = false;
            this.tag = "Untagged";
        }
    }
}
