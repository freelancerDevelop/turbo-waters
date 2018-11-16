using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class UiHudStats : UiWidget
{
    public TextMeshProUGUI scoreLabelText;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        EventManager.Instance.AddListener<PlayerPointsUpdatedEvent>(this.OnPlayerPointsUpdated);
        EventManager.Instance.AddListener<PlayerKillsUpdatedEvent>(this.OnPlayerKillsUpdated);

        this.Show();
    }

    public void OnDestroy()
    {
        if (EventManager.InstanceExists()) {
            EventManager.Instance.RemoveListener<PlayerPointsUpdatedEvent>(this.OnPlayerPointsUpdated);
            EventManager.Instance.RemoveListener<PlayerKillsUpdatedEvent>(this.OnPlayerKillsUpdated);
        }
    }

    public void Render()
    {
        // Check for game runner
        if (this.gameRunner as VanillaRunner == null) {
            this.gameManager = GameManager.Instance;
            this.gameRunner = this.gameManager.GetRunner();
        }

        if (this.gameManager.GetMode() == GameMode.Vanilla) {
            VanillaRunner vanillaRunner = this.gameRunner as VanillaRunner;
            Player player = this.gameRunner.GetPlayer();
            VanillaPlayerStats playerStats = vanillaRunner.GetPlayerStats(player.id);

            if (playerStats != null) {
                this.scoreLabelText.text = "Score: " + playerStats.points.ToString("n0");
            } else {
                this.scoreLabelText.text = "Score: 0";
            }
        }
    }

    private void OnPlayerPointsUpdated(PlayerPointsUpdatedEvent e)
    {
        this.Render();
    }

    private void OnPlayerKillsUpdated(PlayerKillsUpdatedEvent e)
    {
        this.Render();
    }
}
