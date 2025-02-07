using UnityEngine;

public class CardBack : MonoBehaviour
{
    public GameObject cardBack;

    void Update()
    {
        if (gameObject.GetComponent<DisplayCard>().IsCardBack)
        {
            cardBack.SetActive(true);
        }
        else
        {
            cardBack.SetActive(false);
        }
    }
}
