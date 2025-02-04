using System.Collections;
using UnityEngine;

public class CardToHandO : MonoBehaviour
{
    public bool initialized = false;
    public bool played = false;

    public GameObject hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.position = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>().cardInDeckTop.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, 180);
        StartCoroutine(StartAnim());
        initialized = true;
    }

    IEnumerator StartAnim() {
        yield return new WaitUntil(() => gameObject.GetComponent<CardToHandAnim>().m_Initialized);
        gameObject.GetComponent<CardToHandAnim>().StartCardToHandAnim();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<DisplayCard>().cardBack = !played;
    }
}
