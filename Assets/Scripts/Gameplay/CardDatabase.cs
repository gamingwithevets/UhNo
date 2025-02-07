using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();
    [SerializeField] private List<Card> m_Cards;

    void Awake()
    {
        foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
        {
            if (color == CardColor.PINK ||
                color == CardColor.TEAL ||
                color == CardColor.ORANGE ||
                color == CardColor.PURPLE ||
                color == CardColor.WILD) continue;

            string colorName = Enum.GetName(typeof(CardColor), color).ToLower();

            foreach (CardNum num in Enum.GetValues(typeof(CardNum)))
            {
                if (num >= CardNum.DRAW2) break;
                var card = new Card(num, color, Resources.Load<Sprite>("Cards/" + colorName + "_" + (int)num));
                cardList.Add(card);
                if (num == CardNum.N0) continue;
                cardList.Add(card);
            }

            var draw2 = new Card(CardNum.DRAW2, color, Resources.Load<Sprite>("Cards/" + colorName + "_draw2"));
            var reverse = new Card(CardNum.REVERSE, color, Resources.Load<Sprite>("Cards/" + colorName + "_reverse"));
            var skip = new Card(CardNum.SKIP, color, Resources.Load<Sprite>("Cards/" + colorName + "_skip"));
            cardList.Add(draw2);
            cardList.Add(draw2);
            cardList.Add(reverse);
            cardList.Add(reverse);
            cardList.Add(skip);
            cardList.Add(skip);
        }

        // Add wild cards
        var wildColor = new Card(CardNum.COLOR, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_color"));
        var wildDraw4 = new Card(CardNum.DRAW4, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_draw4"));
        cardList.Add(wildColor);
        cardList.Add(wildColor);
        cardList.Add(wildColor);
        cardList.Add(wildColor);
        cardList.Add(wildDraw4);
        cardList.Add(wildDraw4);
        cardList.Add(wildDraw4);
        cardList.Add(wildDraw4);

        m_Cards = cardList;
    }
}
