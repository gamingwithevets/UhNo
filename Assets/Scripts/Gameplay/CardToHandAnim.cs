using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardToHandAnim : MonoBehaviour
{
    public bool initialized = false;
    public bool animating = false;

    public string handName = "Hand";

    public GameObject deckCard;
    public GameObject hand;
    public float animationDuration = 0.5f;
    public Vector3 initialPosition;
    public Vector3 targetPosition;
    public Transform originalParent;

    public void Start() {
        hand = GameObject.Find(handName);

        initialPosition = deckCard.transform.position;
        originalParent = deckCard.transform.parent;

        initialized = true;
    }

    public void Update() {
        if (animating) {
            deckCard.transform.SetParent(hand.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(hand.GetComponent<RectTransform>());
            targetPosition = deckCard.transform.position;
            deckCard.transform.SetParent(originalParent);
            deckCard.transform.position = initialPosition;
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
        animating = true;
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);

            deckCard.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        deckCard.transform.SetParent(hand.transform);
        animating = false;
    }

    private IEnumerator AnimateObject2()
    {
        deckCard.transform.SetParent(originalParent);
        initialPosition = deckCard.transform.position;
        targetPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);

            deckCard.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        Card realCard = deckCard.GetComponent<DisplayCard>().displayCard;
        GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>().discardPile.Add(realCard);
        if (realCard.num == CardNum.COLOR || realCard.num == CardNum.DRAW4) GameObject.Find("TurnSystem").GetComponent<TurnSystem>().SetWildTurn(realCard.color);
        Destroy(deckCard);
    }
}
