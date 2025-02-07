using System.Collections;
using System.Linq;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{

    public bool active = false;
    public bool pickedColor = false;

    public static ColorPicker GetInstance()
    {
        return GameObject.Find("ColorPicker").GetComponent<ColorPicker>();
    }

    public void Reset()
    {
        active = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PickColor(bool red = true, bool blue = true, bool green = true, bool yellow = true)
    {
        pickedColor = false;
        active = true;

        transform.SetAsLastSibling();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(red);
        transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(blue);
        transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(green);
        transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(yellow);
        Debug.Log("Color picker active");
    }

    public void Pick(int color)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        PlayerDeck.GetInstance().discardPile.Last().color = (CardColor)color;
        TurnSystem.GetInstance().SetWildTurn((CardColor)color);
        active = false;
        Debug.Log("Picked color");
        pickedColor = true;
    }
}
