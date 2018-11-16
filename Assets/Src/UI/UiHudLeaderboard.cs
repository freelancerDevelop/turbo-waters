using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class UiHudLeaderboard : UiWidget
{
    private GameManager gameManager;
    private GameModeRunner gameRunner;

    private float nextUpdateIn = 0.02f;
    private List<UiHudLeaderboardEntry> leaderboardEntries = new List<UiHudLeaderboardEntry>();

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        if (this.gameManager.GetMode() == GameMode.Vanilla) {
            this.Show();
        } else {
            this.Hide();
        }
    }

    public void Update()
    {

        // Next update cooldown
        this.nextUpdateIn -= Time.deltaTime;

        // Check if should be updating
        if (this.nextUpdateIn > 0) {
            return;
        }

        // Reset nextUpdate
        this.nextUpdateIn = 1f;

        // Prevent null issue with VanillaRunner
        if (this.gameRunner == null) {
            this.gameManager = GameManager.Instance;
            this.gameRunner = this.gameManager.GetRunner();
        }

        if (this.gameManager.GetMode() == GameMode.Vanilla) {
            VanillaRunner vanillaRunner = this.gameRunner as VanillaRunner;

            if (vanillaRunner.GetAllPlayerStats() == null) {
                return;
            }

            var sortedPlayerStats = vanillaRunner.GetAllPlayerStats().Values.OrderByDescending(player => player.points);
            int i = 0;

            foreach (VanillaPlayerStats player in sortedPlayerStats) {
                if (i >= 5 && player.id != vanillaRunner.GetPlayer().id) {
                    continue;
                }

                UiHudLeaderboardEntry leaderboardEntry;

                if (i < this.leaderboardEntries.Count) {
                    leaderboardEntry = this.leaderboardEntries[i];
                } else {
                    GameObject leaderboardObject = Instantiate(Resources.Load<GameObject>("Prefabs/HUD/LeaderboardEntry"), this.transform);

                    leaderboardEntry = leaderboardObject.AddComponent<UiHudLeaderboardEntry>();
                    leaderboardEntry.Init();

                    this.leaderboardEntries.Add(leaderboardEntry);
                }

                RectTransform leaderboardRect = leaderboardEntry.GetRectTransform();

                leaderboardEntry.SetPlayerId(player.id);
                leaderboardEntry.SetRank(i + 1);
                leaderboardEntry.SetName(player.name);
                leaderboardEntry.SetPoints(player.points);
                leaderboardEntry.Render();

                if (i == 0) {
                    leaderboardRect.localScale = Vector3.one;
                    leaderboardRect.anchoredPosition = Vector3.zero;
                } else if (i == 1) {
                    leaderboardRect.localScale = Vector3.one * 0.9f;
                    leaderboardRect.anchoredPosition = new Vector3(0, -42f, 0);
                } else if (i == 2) {
                    leaderboardRect.localScale = Vector3.one * 0.8f;
                    leaderboardRect.anchoredPosition = new Vector3(0, -80f, 0);
                } else if (i == 3) {
                    leaderboardRect.localScale = Vector3.one * 0.7f;
                    leaderboardRect.anchoredPosition = new Vector3(0, -114f, 0);
                } else if (i == 4) {
                    leaderboardRect.localScale = Vector3.one * 0.6f;
                    leaderboardRect.anchoredPosition = new Vector3(0, -144f, 0);
                } else {
                    leaderboardRect.localScale = Vector3.one * 0.5f;
                    leaderboardRect.anchoredPosition = new Vector3(0, -170f, 0);
                }

                i++;
            }

            if (i < this.leaderboardEntries.Count) {
                for (int j = i; j < this.leaderboardEntries.Count; j++) {
                    Destroy(this.leaderboardEntries[j].gameObject);
                }

                this.leaderboardEntries.RemoveRange(i, this.leaderboardEntries.Count - i);
            }
        }
    }
}
