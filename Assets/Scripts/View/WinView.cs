using UnityEngine;

public class WinView : View
{
    public void OnButtonAgain()
    {
        GameManager.Instance.ResetGame();
        BackView();
    }

    public void OnButtonMenu()
    {
        ViewManager.Instance.ShowView(ViewId.MainMenuView);
    }
}
