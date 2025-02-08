using System.Collections;
using UnityEngine;

public class CardToHandO : MonoBehaviour
{
    public bool initialized = false;
    public bool played = false;
    public GameObject hand;

    void Start()
    {
        transform.SetParent(FindFirstObjectByType<GameplayView>().transform);
        transform.position = PlayerDeck.GetInstance().cardInDeckTop.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, 180);
        StartCoroutine(StartAnim());
        initialized = true;
    }

    IEnumerator StartAnim()
    {
        yield return new WaitUntil(() => gameObject.GetComponent<CardToHandAnim>().m_Initialized);
        gameObject.GetComponent<CardToHandAnim>().StartCardToHandAnim();
    }

    void Update()
    {
        gameObject.GetComponent<DisplayCard>().IsCardBack = !played;
    }
}

