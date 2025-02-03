using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

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
                cardList.Add(new Card(num, color, Resources.Load<Sprite>("Cards/" + colorName + "_" + (int)num)));
                if (num != CardNum.N0) cardList.Add(new Card(num, color, Resources.Load<Sprite>("Cards/" + colorName + "_" + (int)num)));
            }


            cardList.Add(new Card(CardNum.DRAW2, color, Resources.Load<Sprite>("Cards/" + colorName + "_draw2")));
            cardList.Add(new Card(CardNum.DRAW2, color, Resources.Load<Sprite>("Cards/" + colorName + "_draw2")));
            cardList.Add(new Card(CardNum.REVERSE, color, Resources.Load<Sprite>("Cards/" + colorName + "_reverse")));
            cardList.Add(new Card(CardNum.REVERSE, color, Resources.Load<Sprite>("Cards/" + colorName + "_reverse")));
            cardList.Add(new Card(CardNum.SKIP, color, Resources.Load<Sprite>("Cards/" + colorName + "_skip")));
            cardList.Add(new Card(CardNum.SKIP, color, Resources.Load<Sprite>("Cards/" + colorName + "_skip")));


        }

        // Add wild cards
        cardList.Add(new Card(CardNum.COLOR, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_color")));
        cardList.Add(new Card(CardNum.COLOR, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_color")));
        cardList.Add(new Card(CardNum.COLOR, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_color")));
        cardList.Add(new Card(CardNum.COLOR, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_color")));
        cardList.Add(new Card(CardNum.DRAW4, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_draw4")));
        cardList.Add(new Card(CardNum.DRAW4, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_draw4")));
        cardList.Add(new Card(CardNum.DRAW4, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_draw4")));
        cardList.Add(new Card(CardNum.DRAW4, CardColor.WILD, Resources.Load<Sprite>("Cards/wild_draw4")));
    }
}
