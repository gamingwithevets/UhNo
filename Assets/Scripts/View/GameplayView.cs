using UnityEngine;

public class GameplayView : View
{
    protected override void OnShow()
    {
        GameManager.Instance.InitGame();
    }
}
