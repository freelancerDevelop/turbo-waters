using UnityEngine;
using TMPro;

public class UiHudPips : UiWidget
{
    private GameManager gameManager;
    private GameModeRunner gameRunner;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        EventManager.Instance.AddListener<PlayerPointsUpdatedEvent>(this.OnPlayerPointsUpdated);
        EventManager.Instance.AddListener<PlayerTierUpdatedEvent>(this.OnPlayerTierUpdated);
    }

    public void OnDestroy()
    {
        if (EventManager.InstanceExists()) {
            EventManager.Instance.RemoveListener<PlayerPointsUpdatedEvent>(this.OnPlayerPointsUpdated);
        }
    }

    private void OnPlayerPointsUpdated(PlayerPointsUpdatedEvent e)
    {
        if (!e.player.isLocalPlayer) {
            return;
        }

        if (e.gained >= 2f) {
            GameObject pipObject = Instantiate(Resources.Load<GameObject>("Prefabs/HUD/Pip"), this.transform);
            UiHudPip pip = pipObject.AddComponent<UiHudPip>();

            pip.Init();
            pip.SetText("+" + (int) e.gained);
            pip.SetDuration(0.5f);
            pip.Render();
        }
    }

    private void OnPlayerTierUpdated(PlayerTierUpdatedEvent e) {
        if (!e.player.isLocalPlayer) {
            return;
        }


    }
}
