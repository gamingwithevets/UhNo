using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    // Singleton instance for global access
    public static FadeManager Instance { get; private set; }

    // Public properties for fade durations
    public float FadeTimeIn => _fadeTimeIn;
    public float FadeTimeOut => _fadeTimeOut;
    public bool IsFading => _isFading;

    // Private fields
    private Image _fadeImage;
    private float _fadeTimeIn = 0.5f;
    private float _fadeTimeOut = 0.5f;
    private bool _isFading;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the fade image
        _fadeImage = GetComponent<Image>();
        if (_fadeImage == null)
        {
            _fadeImage = gameObject.AddComponent<Image>();
        }

        // Default setup for the fade image
        _fadeImage.color = Color.clear; // Start with fully transparent
        _fadeImage.raycastTarget = false; // Disable raycast blocking
        gameObject.SetActive(false); // Initially inactive
    }

    /// <summary>
    /// Fades in the screen over a specified duration.
    /// </summary>
    /// <param name="fadeTime">Duration of the fade-in effect.</param>
    /// <returns>The duration of the fade-in effect.</returns>
    public float FadeIn(float fadeTime = 1)
    {
        Debug.Log($"Fade In <color=blue>●</color>");
        _fadeTimeIn = fadeTime;

        if (_isFading) return _fadeTimeIn; // Prevent overlapping fades

        _isFading = true;
        gameObject.SetActive(true);

        _fadeImage.DOFade(0, _fadeTimeIn)
            .From(1)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                _fadeImage.gameObject.SetActive(false);
                _isFading = false;
            });

        return _fadeTimeIn;
    }

    /// <summary>
    /// Fades out the screen over a specified duration.
    /// </summary>
    /// <param name="fadeTime">Duration of the fade-out effect.</param>
    /// <returns>The duration of the fade-out effect.</returns>
    public float FadeOut(float fadeTime = 0.5f)
    {
        Debug.Log($"Fade Out <color=blue>●</color>");
        _fadeTimeOut = fadeTime;

        if (_isFading) return _fadeTimeOut; // Prevent overlapping fades

        _isFading = true;
        gameObject.SetActive(true);

        _fadeImage.DOFade(1, _fadeTimeOut)
            .From(0)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                _fadeImage.gameObject.SetActive(false);
                _isFading = false;
            });

        return _fadeTimeOut;
    }

    /// <summary>
    /// Immediately cancels any ongoing fade effects.
    /// </summary>
    public void CancelFade()
    {
        DOTween.Kill(_fadeImage); // Stop all tweens on the fade image
        _fadeImage.gameObject.SetActive(false);
        _isFading = false;
    }
}