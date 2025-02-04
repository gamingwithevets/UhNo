using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class View : MonoBehaviour
{
    private bool _firstShow = true;
    public bool IsShown { get; protected set; }

    private void OnValidate()
    {
#if UNITY_EDITOR
        name = GetType().Name; // Automatically set the GameObject's name to the class name in the editor.
#endif
    }

    protected virtual void Awake()
    {
        if (!IsShown)
        {
            gameObject.SetActive(false);
        }
    }

    public bool Show(bool isFade = false)
    {
        if (IsShown) return false;

        IsShown = true;
        gameObject.SetActive(true);

        if (_firstShow)
        {
            OnFirstShow();
            _firstShow = false;
        }

        OnPreShow();
        StartCoroutine(CoroutineShow(isFade));
        return true;
    }

    public bool Hide(bool isFade = false)
    {
        if (!IsShown) return false;

        IsShown = false;
        StartCoroutine(CoroutineHide(isFade));
        return true;
    }

    protected virtual void OnFirstShow() { }
    protected virtual void OnPreShow() { }
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnBackButton() { }

    protected virtual IEnumerator CoroutineShow(bool isFade)
    {
        yield return CoroutineShowAnim(isFade);
        if (IsShown) OnShow();
    }

    protected virtual IEnumerator CoroutineHide(bool isFade)
    {
        yield return CoroutineHideAnim(isFade);
        if (!IsShown)
        {
            OnHide();
            gameObject.SetActive(false);
            Debug.Log($"Hide <color=yellow>●</color> {name}");
        }
    }

    protected virtual IEnumerator CoroutineShowAnim(bool isFade)
    {
        if (isFade)
        {
            var fadeManager = GetFadeManager();
            float fadeTime = fadeManager.FadeTimeIn;
            Debug.Log($"Fade In <color=blue>●</color> {fadeTime}");
            yield return new WaitForSeconds(fadeTime);
        }
    }

    protected virtual IEnumerator CoroutineHideAnim(bool isFade)
    {
        if (isFade)
        {
            var fadeManager = GetFadeManager();
            float fadeTime = fadeManager.FadeTimeOut;
            Debug.Log($"Fade Out <color=blue>●</color> {fadeTime}");
            yield return new WaitForSeconds(fadeTime);
        }
    }

    private FadeManager GetFadeManager()
    {
        var fadeManager = FadeManager.Instance;
        if (fadeManager == null)
        {
            Debug.LogWarning("FadeManager not found. Attempting to locate...");
            fadeManager = FindObjectOfType<FadeManager>();
            if (fadeManager == null)
            {
                Debug.LogError("FadeManager is missing in the scene.");
            }
        }
        fadeManager.gameObject.SetActive(true);
        return fadeManager;
    }

    public void BackView()
    {
        ViewManager.Instance.BackView();
    }
}