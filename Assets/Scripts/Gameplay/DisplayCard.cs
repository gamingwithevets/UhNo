using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour
{
    [SerializeField] private bool m_Initialized = false;
    [SerializeField] private Card m_CardInfo;
    [SerializeField] private CardNum m_Num;
    [SerializeField] private CardColor m_Color;
    [SerializeField] private Image m_Image;
    [SerializeField] private GameObject m_CardBack;
    [Header("Read Only")]
    [SerializeField] private bool m_IsCardBack;
    [SerializeField] private GameObject m_HandOfPlayer;
    [SerializeField] private int m_NumberOfCardsInDeck;

    public bool Initialized => m_Initialized;
    public Card CardInfo
    {
        get => m_CardInfo;
        set => m_CardInfo = value;
    }

    public bool IsCardBack
    {
        get => m_IsCardBack;
        set => m_IsCardBack = value;
    }

    void Start()
    {
        m_NumberOfCardsInDeck = PlayerDeck.deckSize;
        m_Initialized = true;
    }

    void Update()
    {
        m_Num = CardInfo.num;
        m_Color = CardInfo.color;
        m_Image.sprite = CardInfo.sprite;

        if (m_IsCardBack) HideCard();
        else ShowCard();

        if (tag == "Clone")
        {
            m_CardInfo = PlayerDeck.staticDeck[m_NumberOfCardsInDeck - 1];
            --m_NumberOfCardsInDeck;
            --PlayerDeck.deckSize;
            if (PlayerDeck.deckSize == 0) PlayerDeck.GetInstance().RefillDeck();
            m_IsCardBack = false;
            tag = "Untagged";
        }
    }

    public void ShowCard()
    {
        m_CardBack.SetActive(false);
    }

    public void HideCard()
    {
        m_CardBack.SetActive(true);
    }
}
