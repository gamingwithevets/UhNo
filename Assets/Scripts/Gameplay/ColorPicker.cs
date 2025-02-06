using System.Linq;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{

    public bool active = false;

    public static ColorPicker GetInstance() {
        return GameObject.Find("ColorPicker").GetComponent<ColorPicker>();
    }

    public void Reset() {
        active = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PickColor(bool red = true, bool blue = true, bool green = true, bool yellow = true) {
        active = true;

        transform.SetAsLastSibling();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(red);
        transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(blue);
        transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(green);
        transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(yellow);
    }

    public void Pick(int color) {
        transform.GetChild(0).gameObject.SetActive(false);
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        playerDeck.discardPile.Last().color = (CardColor)color;
        TurnSystem turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
        turnSystem.SetWildTurn((CardColor)color);
        active = false;
    }
}
