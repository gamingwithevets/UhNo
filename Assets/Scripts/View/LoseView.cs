using UnityEngine;

public class LoseView : View
{
    public void OnButtonAgain()
    {
        GameManager.Instance.ResetGame();
        BackView();
    }
}
