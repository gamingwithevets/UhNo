using UnityEngine;

public class MainMenuView : View
{
    public void OnButtonStart()
    {
        ViewManager.Instance.ShowView(ViewId.GameplayView);
    }

    public void OnButtonSettings()
    {
        ViewManager.Instance.ShowView(ViewId.SettingsView);
    }

    public void OnButtonExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
