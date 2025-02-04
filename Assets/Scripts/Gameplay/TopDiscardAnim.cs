using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopDiscardAnim : MonoBehaviour
{
    public bool initialized = false;

    public GameObject deckCard;
    public float animationDuration = 0.5f;
    public Vector3 initialPosition;
    public Vector3 targetPosition;

    public void Start() {
        initialPosition = deckCard.transform.position;

        initialized = true;
    }

    public void StartAnim() {
        StartCoroutine(AnimateObject());
    }

    private IEnumerator AnimateObject()
    {
        targetPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);

            deckCard.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }
    }
}
