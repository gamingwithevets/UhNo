using UnityEngine;

public class SettingsView : View
{
    public void OnButtonBack()
    {
        ViewManager.Instance.ShowView(ViewId.MainMenuView);
    }
}
