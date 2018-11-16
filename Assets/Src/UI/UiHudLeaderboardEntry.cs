using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UiHudLeaderboardEntry : MonoBehaviour
{
    private RectTransform rectTransform;
    private ProceduralImage backgroundImage;
    private ProceduralImage rankImage;
    private TextMeshProUGUI rankLabelText;
    private TextMeshProUGUI nameLabelText;
    private TextMeshProUGUI pointsLabelText;

    private uint playerId = 0;
    private int rank = 1;
    private string name = "Player";
    private int points = 0;

    public void Init()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
        this.backgroundImage = this.GetComponent<ProceduralImage>();
        this.nameLabelText = this.transform.Find("NameLabel").GetComponent<TextMeshProUGUI>();
        this.pointsLabelText = this.transform.Find("PointsLabel").GetComponent<TextMeshProUGUI>();

        if (this.transform.Find("EntryRank") != null) {
            this.rankImage = this.transform.Find("EntryRank").GetComponent<ProceduralImage>();
            this.rankLabelText = this.transform.Find("EntryRank/RankLabel").GetComponent<TextMeshProUGUI>();
        }
    }

    public RectTransform GetRectTransform()
    {
        return this.rectTransform;
    }

    public void SetPlayerId(uint playerId)
    {
        this.playerId = playerId;
    }

    public void SetRank(int rank)
    {
        this.rank = rank;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public void SetPoints(int points)
    {
        this.points = points;
    }

    public void Render()
    {
        if (this.playerId == GameManager.Instance.GetRunner().GetPlayer().id) {
            this.backgroundImage.color = new Color32(50, 136, 236, 255);
        } else {
            this.backgroundImage.color = new Color32(255, 255, 255, 30);
        }

        this.nameLabelText.text = this.name;
        this.pointsLabelText.text = this.points.ToString("n0");

        if (this.rankLabelText != null) {
            this.rankLabelText.text = this.rank.ToString("n0");
        }
    }
}
