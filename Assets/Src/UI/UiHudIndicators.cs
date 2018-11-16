using UnityEngine;
using TMPro;

public class UiHudIndicators : UiWidget
{
    private GameManager gameManager;
    private GameModeRunner gameRunner;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();
    }
}
