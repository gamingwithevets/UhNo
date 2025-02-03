using UnityEngine;

public class CardToHandO : MonoBehaviour
{
    public bool initialized = false;

    public GameObject hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<DisplayCard>().cardBack = true;
        hand = GameObject.Find("HandO");
        gameObject.transform.SetParent(hand.transform);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
    }
}
