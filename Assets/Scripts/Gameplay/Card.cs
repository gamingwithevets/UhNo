using UnityEngine;

public enum CardNum
{
    N0, N1, N2, N3, N4, N5, N6, N7, N8, N9,
    DRAW2, DRAW4, DRAW5, DRAW_COLOR,
    REVERSE, SKIP, SKIPALL,
    COLOR, FLIP
}

public enum CardColor
{
    RED, BLUE, GREEN, YELLOW,
    PINK, TEAL, ORANGE, PURPLE,
    WILD
}

[System.Serializable]
public class Card
{
    public CardNum num;
    public CardColor color;
    public Sprite sprite;

    public Card() { }

    public Card(CardNum Num, CardColor Color, Sprite Sprite)
    {
        num = Num;
        color = Color;
        sprite = Sprite;
    }
}
