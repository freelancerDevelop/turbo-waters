using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class UiGameOver : UiLayer
{
    public GameObject leaderboardContainer;
    public TextMeshProUGUI helpLabel;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    private List<UiHudLeaderboardEntry> leaderboardEntries = new List<UiHudLeaderboardEntry>();

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        if (this.gameManager.GetMode() == GameMode.Vanilla) {
            VanillaRunner vanillaRunner = this.gameRunner as VanillaRunner;
            IEnumerable<VanillaPlayerStats> sortedPlayerStats = vanillaRunner.GetAllPlayerStats().Values.OrderByDescending(player => player.points);
            int i = 0;

            this.revealer.steps.Clear();

            foreach (VanillaPlayerStats player in sortedPlayerStats) {
                if (i >= 5) {
                    break;
                }

                GameObject leaderboardObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameOver/LeaderboardEntry"), this.leaderboardContainer.transform);
                UiHudLeaderboardEntry leaderboardEntry = leaderboardObject.AddComponent<UiHudLeaderboardEntry>();
                RectTransform leaderboardRect = leaderboardObject.GetComponent<RectTransform>();

                leaderboardEntry.Init();
                leaderboardEntry.SetPlayerId(player.id);
                leaderboardEntry.SetRank(i + 1);
                leaderboardEntry.SetName(player.name);
                leaderboardEntry.SetPoints(player.points);
                leaderboardEntry.Render();

                leaderboardRect.anchoredPosition = new Vector3(0, -i * 50f, 0);

                this.leaderboardEntries.Add(leaderboardEntry);

                this.revealer.steps.Add(new RevealerStep {
                    canvasGroup = leaderboardObject.GetComponent<CanvasGroup>(),
                    direction = RevealerDirection.Right,
                    duration = 0.5f,
                    nextDelay = 0.5f
                });

                i++;
            }

            this.revealer.steps.Add(new RevealerStep {
                canvasGroup = this.helpLabel.GetComponent<CanvasGroup>(),
                direction = RevealerDirection.Up,
                duration = 0.5f,
                nextDelay = 0
            });

            uint currentPlayerId = vanillaRunner.GetPlayer().id;
            VanillaPlayerStats currentPlayer = vanillaRunner.GetPlayerStats(currentPlayerId);

            StorageManager.Instance.SetInt(StorageKeys.AllRounds, StorageManager.Instance.GetInt(StorageKeys.AllRounds, 0) + 1);
            StorageManager.Instance.SetInt(StorageKeys.VanillaRounds, StorageManager.Instance.GetInt(StorageKeys.VanillaRounds, 0) + 1);
            StorageManager.Instance.SetInt(StorageKeys.VanillaTotalScore, StorageManager.Instance.GetInt(StorageKeys.VanillaTotalScore, 0) + currentPlayer.points);
            StorageManager.Instance.SetInt(StorageKeys.VanillaTotalKills, StorageManager.Instance.GetInt(StorageKeys.VanillaTotalKills, 0) + currentPlayer.kills);

            if (currentPlayer.points > StorageManager.Instance.GetInt(StorageKeys.VanillaHighScore)) {
                StorageManager.Instance.SetInt(StorageKeys.VanillaHighScore, currentPlayer.points);
            }

            if (currentPlayer.kills > StorageManager.Instance.GetInt(StorageKeys.VanillaHighKills)) {
                StorageManager.Instance.SetInt(StorageKeys.VanillaHighKills, currentPlayer.kills);
            }

            int playerXp = StorageManager.Instance.GetInt(StorageKeys.PlayerXP, 0);
            int playerLevel = StorageManager.Instance.GetInt(StorageKeys.PlayerLevel, 1);
            int experienceGained = (int) (currentPlayer.points / 20f);

            playerXp += experienceGained;

            int newLevel = this.GetLevelForExperience(playerXp);

            if (newLevel > playerLevel) {
                // Levelled up!
            }

            StorageManager.Instance.SetInt(StorageKeys.PlayerXP, playerXp);
            StorageManager.Instance.SetInt(StorageKeys.PlayerLevel, newLevel);

            this.revealer.Reveal();
        }
    }

    public void OnTriggerMainMenu()
    {
        TransitionManager.Instance.LoadScene("MainMenuScene");
    }

    public void OnTriggerRestart()
    {
        TransitionManager.Instance.LoadScene("GameScene");

        this.gameManager.StartRound();
    }

    private int GetLevelForExperience(int experience)
    {
        return 1 + (int) Mathf.Floor(experience / 100f);
    }
}
