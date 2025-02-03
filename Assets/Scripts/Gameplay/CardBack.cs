using UnityEngine;

public class CardBack : MonoBehaviour
{
    public GameObject cardBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DisplayCard.staticCardBack) {
            cardBack.SetActive(true);
        } else {
            cardBack.SetActive(false);
        }
    }
}

