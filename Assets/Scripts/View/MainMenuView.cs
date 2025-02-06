using UnityEngine;

public class MainMenuView : View
{
    public void OnButtonStart()
    {
        ViewManager.Instance.ShowView(ViewId.ModeSelectView);
    }

    public void OnButtonSettings()
    {
        ViewManager.Instance.ShowView(ViewId.SettingsView);
    }

    public void OnButtonGitHub()
    {
        Application.OpenURL("https://github.com/gamingwithevets/UhNo");
    }

}
