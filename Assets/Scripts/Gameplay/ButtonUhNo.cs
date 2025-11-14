using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class ButtonUhNo : MonoBehaviour
{
    public static ButtonUhNo Instance { get; private set; }

    private Queue<Action> uhNoQueue = new Queue<Action>();

    private bool uhNoPressed = false;
    private Action currentTimeoutCallback;
    private Action currentSuccessCallback;

    void Awake() {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    public void Update()
    {
        gameObject.GetComponent<Image>().color = PlayerDeck.uhNoActive ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
        gameObject.GetComponent<Button>().interactable = PlayerDeck.uhNoActive;

        if (uhNoQueue.Count > 0)
        {
            Action startAction = uhNoQueue.Dequeue();
            startAction?.Invoke();
        }
    }

    public void RequestUhNoTimer(Action onTimeout, Action onSuccess = null)
    {
        uhNoQueue.Enqueue(() => StartUhNoInternal(onTimeout, onSuccess));
    }

    private void StartUhNoInternal(Action onTimeout, Action onSuccess)
    {
        if (!PlayerDeck.uhNoActive) return;

        uhNoPressed = false;
        currentTimeoutCallback = onTimeout;
        currentSuccessCallback = onSuccess;

        gameObject.GetComponent<Button>().onClick.AddListener(OnUhNoPressedOnce);

        StartCoroutine(WaitUhNo());
    }

    private void OnUhNoPressedOnce() {
        if (!PlayerDeck.uhNoActive) return;

        uhNoPressed = true;
        currentSuccessCallback?.Invoke();
        gameObject.GetComponent<Button>().onClick.RemoveListener(OnUhNoPressedOnce);
    }

    IEnumerator WaitUhNo() {
        float endTime = Time.realtimeSinceStartup + 2f;
        while (Time.realtimeSinceStartup < endTime)
        {
            if (this == null || !gameObject.activeInHierarchy)
            {
                Debug.LogWarning("[ButtonUhNo] Manager dead, cannot continue!");
                yield break;
            }

            if (uhNoPressed) yield break;
            yield return null;
        }

        if (PlayerDeck.uhNoActive) currentTimeoutCallback?.Invoke();
        EndUhNo();
    }

    private void EndUhNo()
    {
        PlayerDeck.uhNoActive = false;
        uhNoPressed = false;
        gameObject.GetComponent<Button>().onClick.RemoveListener(OnUhNoPressedOnce);
    }

    public void ActivateUhNo()
    {
        PlayerDeck.uhNoActive = true;
    }
}
