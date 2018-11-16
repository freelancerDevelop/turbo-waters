using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class UiHudRank : UiWidget
{
    public TextMeshProUGUI rankLabelText;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    private float nextUpdateIn = 0;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        this.nextUpdateIn = 0;

        if (this.gameManager.GetMode() == GameMode.Vanilla) {
            this.Show();
        } else {
            this.Hide();
        }
    }

    public void Update() {

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        this.nextUpdateIn -= Time.deltaTime;

        if (this.nextUpdateIn > 0) {
            return;
        }

        this.nextUpdateIn = 0.5f;

        if (this.gameManager.GetMode() == GameMode.Vanilla) {
            VanillaRunner vanillaRunner = this.gameRunner as VanillaRunner;

            if (vanillaRunner != null && vanillaRunner.GetAllPlayerStats() == null) {
                return;
            }

            var sortedPlayerStats = vanillaRunner.GetAllPlayerStats().Values.OrderByDescending(player => player.points);
            int rank = 0;

            foreach (VanillaPlayerStats player in sortedPlayerStats) {
                rank++;

                if (player.id != vanillaRunner.GetPlayer().id) {
                    continue;
                }

                this.rankLabelText.text = "<size=32><color=#FFFFFF99>#</color></size>" + rank;
            }
        }
    }
}
