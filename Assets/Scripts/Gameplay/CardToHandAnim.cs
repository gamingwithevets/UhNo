using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardToHandAnim : MonoBehaviour
{
    public bool m_Initialized = false;
    public bool m_Animating = false;
    public bool m_Animated = false;
    public bool m_Destroyed = false;

    public string m_HandName = "Hand";

    public GameObject m_DeckCard;
    public GameObject m_Hand;
    public float animationDuration = 0.5f;
    public Vector3 initialPosition;
    public Vector3 targetPosition;
    public Transform originalParent;

    public void Start() {
        m_Hand = GameObject.Find(m_HandName);

        initialPosition = m_DeckCard.transform.position;
        originalParent = m_DeckCard.transform.parent;

        m_Initialized = true;
    }

    public void Update() {
        if (m_Animating) {
            m_DeckCard.transform.SetParent(m_Hand.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Hand.GetComponent<RectTransform>());
            targetPosition = m_DeckCard.transform.position;
            m_DeckCard.transform.SetParent(originalParent);
            m_DeckCard.transform.position = initialPosition;
        }
    }

    public void StartCardToHandAnim() {
        StartCoroutine(AnimateObject());
    }

    public void StartPlayAnim() {
        StartCoroutine(AnimateObject2());
    }

    private IEnumerator AnimateObject()
    {
        m_Animating = true;
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);

            m_DeckCard.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        m_DeckCard.transform.SetParent(m_Hand.transform);
        m_Animating = false;
        m_Animated = true;
    }

    private IEnumerator AnimateObject2()
    {
        yield return new WaitUntil(() => m_Animated);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_Hand.GetComponent<RectTransform>());
        initialPosition = m_DeckCard.transform.position;
        m_DeckCard.transform.SetParent(originalParent);
        targetPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);

            m_DeckCard.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        Card realCard = m_DeckCard.GetComponent<DisplayCard>().displayCard;
        PlayerDeck.GetInstance().discardPile.Add(realCard);
        if (realCard.num == CardNum.COLOR || realCard.num == CardNum.DRAW4) TurnSystem.GetInstance().SetWildTurn(realCard.color);
        Destroy(m_DeckCard);
        m_Destroyed = true;
    }
}
