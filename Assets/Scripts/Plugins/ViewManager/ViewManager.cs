using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    [SerializeField] private Transform _defaultParent;
    [SerializeField] private ViewId _defaultViewId;

    [Header("Read Only")]
    [SerializeField] private List<View> _views = new();
    [SerializeField] private List<View> _viewOrder = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _views.Clear();
        foreach (Transform child in _defaultParent)
        {
            if (child.TryGetComponent(out View view))
            {
                _views.Add(view);
            }
        }

        HideAllViews();
        ShowView(_defaultViewId.ToString());
    }

    public void ShowView(ViewId viewId, bool isSingleView = true, bool isFade = false)
    {
        ShowView(viewId.ToString(), isSingleView, isFade);
    }

    public void ShowView(string viewName, bool isSingleView = true, bool isFade = false)
    {
        var view = _views.Find(v => v.name == viewName);
        if (view == null)
        {
            Debug.LogError($"View '{viewName}' not found.");
            return;
        }

        if (view.IsShown) return;

        if (CheckIfAnyViewIsShown())
        {
            if (isFade)
            {
                StartCoroutine(ShowViewWithFadeCoroutine(viewName, isSingleView));
                return;
            }

            if (isSingleView)
            {
                HideAllViews(false);
            }
        }

        view.Show(isFade);
        Debug.Log($"Show <color=green>●</color> {viewName}");
        _viewOrder.Add(view);
    }

    private IEnumerator ShowViewWithFadeCoroutine(string viewName, bool isSingleView)
    {
        float fadeOutTime = FadeManager.Instance.FadeOut();
        if (isSingleView) HideAllViews(true);
        yield return new WaitForSeconds(fadeOutTime);

        float fadeInTime = FadeManager.Instance.FadeIn();
        ShowView(viewName, isSingleView, true);
        yield return new WaitForSeconds(fadeInTime);
    }

    public void HideView(string viewName, bool isFade = false)
    {
        var view = _views.Find(v => v.name == viewName);
        if (view != null) view.Hide(isFade);
    }

    public void HideAllViews(bool isFade = false)
    {
        Debug.Log($"Hide All Views <color=red>●</color> fade: {isFade}");
        foreach (var view in _views)
        {
            view.Hide(isFade);
        }
    }

    public bool CheckIfAnyViewIsShown()
    {
        return _views.Exists(view => view.IsShown);
    }

    public View BackView()
    {
        if (_viewOrder.Count < 2)
        {
            Debug.LogWarning("No previous view to return to.");
            return null;
        }

        var lastView = _viewOrder[^1];
        lastView.Hide();
        _viewOrder.RemoveAt(_viewOrder.Count - 1);

        var previousView = _viewOrder[^1];
        previousView.Show();
        return previousView;
    }
}