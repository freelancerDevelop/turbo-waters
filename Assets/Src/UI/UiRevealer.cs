using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum RevealerDirection
{
    None = 0,
    Down = 1,
    Up = 2,
    Left = 3,
    Right = 4
}

[Serializable]
public class RevealerStep
{
    public CanvasGroup canvasGroup;
    public RevealerDirection direction = RevealerDirection.None;
    public float duration = 1f;
    public float nextDelay = 1.5f;
}

public class UiRevealer : MonoBehaviour
{
    public List<RevealerStep> steps = new List<RevealerStep>();

    private bool isRevealing = false;
    private int currentIndex = 0;
    private float revealTimer = 0;
    private List<RectTransform> stepRects = new List<RectTransform>();
    private List<Vector3> initialPositions = new List<Vector3>();

    public void Start()
    {
        for (int i = 0; i < this.steps.Count; i++) {
            this.stepRects.Add(this.steps[i].canvasGroup.GetComponent<RectTransform>());
            this.initialPositions.Add(this.stepRects[i].anchoredPosition);
        }
    }

    public void Update()
    {
        if (!this.isRevealing) {
            return;
        }

        this.revealTimer -= Time.deltaTime;

        if (this.revealTimer > 0) {
            return;
        }

        this.currentIndex++;
        this.revealTimer = this.steps[this.currentIndex].nextDelay;

        this.RevealGroup(this.currentIndex);

        if (this.currentIndex == this.steps.Count - 1) {
            this.isRevealing = false;
        }
    }

    public void Reveal()
    {
        if (this.isRevealing) {
            return;
        }

        if (this.initialPositions.Count == 0) {
            this.Start();
        }

        for (int i = 0; i < this.steps.Count; i++) {
            AnimationManager.Instance.Hide(this.steps[i].canvasGroup);
        }

        this.isRevealing = true;
        this.currentIndex = 0;
        this.revealTimer = this.steps[this.currentIndex].nextDelay;

        this.RevealGroup(this.currentIndex);
    }

    public void RevealAfter(float delay)
    {
        if (this.isRevealing) {
            return;
        }

        if (this.initialPositions.Count == 0) {
            this.Start();
        }

        for (int i = 0; i < this.steps.Count; i++) {
            AnimationManager.Instance.Hide(this.steps[i].canvasGroup);
        }

        AnimationManager.Instance.FireAfterDelay(this.Reveal, delay);
    }

    private void RevealGroup(int i)
    {
        Vector3 initialPosition = this.initialPositions[i];
        Vector3 offsetPosition = initialPosition;

        if (this.steps[i].direction == RevealerDirection.Down) {
            offsetPosition += new Vector3(0, 15f, 0);
        } else if (this.steps[i].direction == RevealerDirection.Up) {
            offsetPosition += new Vector3(0, -15f, 0);
        } else if (this.steps[i].direction == RevealerDirection.Left) {
            offsetPosition += new Vector3(15f, 0, 0);
        } else if (this.steps[i].direction == RevealerDirection.Right) {
            offsetPosition += new Vector3(-15f, 0, 0);
        }

        this.stepRects[i].anchoredPosition = offsetPosition;

        AnimationManager.Instance.MoveTo(this.stepRects[i], initialPosition, this.steps[i].duration, EasingType.EaseOut);
        AnimationManager.Instance.FadeIn(this.steps[i].canvasGroup, this.steps[i].duration);
    }
}
