using System.Collections;
using System.Linq;
using UnityEngine;

public class ConfirmEnd : MonoBehaviour
{
    public static ConfirmEnd Instance { get; private set; }


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

    public void OnButtonEnd()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        TurnSystem.Instance.Lose();
    }
}
