using System.Collections;
using System.Linq;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    public static ColorPicker Instance { get; private set; }

    int cardsToDraw;

    void Awake() {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
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
        transform.GetChild(0).gameObject.SetActive(false);
        PlayerDeck.Instance.NextColor = (CardColor)color;
        TurnSystem.Instance.SetWildTurn((CardColor)color);
        TurnSystem.Instance.PlayerPlayed = true;
        PlayerDeck.cardsToDraw += cardsToDraw;
        if (PlayerDeck.Instance.playerClones.Count == 1) {
            ButtonUhNo.Instance.ActivateUhNo();
            ButtonUhNo.Instance.RequestUhNoTimer(
                onTimeout: () => UhNoPopup.Instance.Show(),
                onSuccess: () => TurnSystem.Instance.EndPlayerTurn()
            );
        }
        else TurnSystem.Instance.EndPlayerTurn();
    }
}
