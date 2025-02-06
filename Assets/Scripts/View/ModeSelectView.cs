using UnityEngine;

public class ModeSelectView : View
{
    public void OnButton1P()
    {
        ViewManager.Instance.ShowView(ViewId.HouseRuleSelectView);
    }

    public void OnButtonBack()
    {
        ViewManager.Instance.ShowView(ViewId.MainMenuView);
    }
}
