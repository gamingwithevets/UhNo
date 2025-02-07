using System.Collections;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{

    int cardsToDraw;

    public static ColorPicker GetInstance()
    {
        return GameObject.Find("ColorPicker").GetComponent<ColorPicker>();
    }

    public void Reset()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PickColor(int CardsToDraw = 0, bool red = true, bool blue = true, bool green = true, bool yellow = true)
    {
        cardsToDraw = CardsToDraw;

        transform.SetAsLastSibling();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(red);
        transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(blue);
        transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(green);
        transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(yellow);
    }

    public void Pick(int color)
    {
        PlayerDeck playerDeck = PlayerDeck.GetInstance();
        TurnSystem turnSystem = TurnSystem.GetInstance();
        transform.GetChild(0).gameObject.SetActive(false);
        playerDeck.discardPile.Last().color = (CardColor)color;
        turnSystem.SetWildTurn((CardColor)color);
        turnSystem.PlayerPlayed = true;
        PlayerDeck.cardsToDraw += cardsToDraw;
        turnSystem.EndPlayerTurn();
    }
}
