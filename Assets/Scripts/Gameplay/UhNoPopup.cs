using System.Collections;
using System.Linq;
using UnityEngine;

public class UhNoPopup : MonoBehaviour
{
    public static UhNoPopup Instance { get; private set; }

    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    public void Reset()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Show()
    {
        transform.SetAsLastSibling();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnButtonOK()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        PlayerDeck.uhNoActive = false;
        PlayerDeck.cardsToDraw += 2;
    }
}
