using UnityEngine;

public class HouseRuleSelectView : View
{
    public void OnButtonStart()
    {
        ViewManager.Instance.ShowView(ViewId.GameplayView);
    }

    public void OnButtonBack()
    {
        ViewManager.Instance.ShowView(ViewId.ModeSelectView);
    }
}
