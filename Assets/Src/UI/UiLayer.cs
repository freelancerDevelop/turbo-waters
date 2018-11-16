using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;

public enum UiLayerAnimationType
{
    None = 0,
    PopInOut = 1,
    DropInOut = 2,
    FadeInOut = 3
}

public enum UiLayerPlatform
{
    Default = 0,
    Desktop = 1,
    Mobile = 2
}

[Serializable]
public class UiLayerPlatformOption
{
    public UiLayerPlatform platform;
    public GameObject platformObject;
}

[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(CanvasGroup))]
public class UiLayer : MonoBehaviour
{
    [Header("Main Options")]
    public bool startVisible = true;
    public bool scaleForDesktop = false;
    public bool shouldSendStats = true;

    [Header("Platform Specific")]
    public List<UiLayerPlatformOption> platformDependent = new List<UiLayerPlatformOption>();

    [Header("Animations")]
    public UiLayerAnimationType animationType = UiLayerAnimationType.None;
    public float animationSpeed = 1f;
    public CanvasGroup backgroundGroup;
    public CanvasGroup overlayGroup;
    public UiRevealer revealer;

    protected CanvasScaler canvasScaler;
    protected CanvasGroup canvasGroup;

    protected float visibleAt = 0;

    public virtual void Start()
    {
        this.canvasScaler = this.GetComponent<CanvasScaler>();
        this.canvasGroup = this.GetComponent<CanvasGroup>();

        #if PLATFORM_DESKTOP
            if (this.scaleForDesktop) {
                this.canvasScaler.referenceResolution = new Vector2(1400f, 840f);
            }
        #endif

        if (!this.startVisible) {
            AnimationManager.Instance.Hide(this.canvasGroup);

            this.gameObject.SetActive(false);
        } else {
            AnimationManager.Instance.Show(this.canvasGroup);

            if (this.shouldSendStats) {
                GameAnalytics.NewDesignEvent("UI:ChangeView:" + this.name);

                this.visibleAt = Time.time;
            }
        }
    }

    public virtual void OnApplicationQuit()
    {
        if (!this.IsVisible()) {
            return;
        }

        if (!this.shouldSendStats) {
            return;
        }

        GameAnalytics.NewDesignEvent("UI:ViewDuration:" + this.name, Time.time - this.visibleAt);
    }

    public virtual bool IsVisible()
    {
        return this.gameObject.activeSelf;
    }

    public virtual void Show()
    {
        if (this.IsVisible()) {
            return;
        }

        this.gameObject.SetActive(true);

        if (this.animationType == UiLayerAnimationType.None) {
            AnimationManager.Instance.Show(this.canvasGroup);

            if (this.revealer != null) {
                this.revealer.Reveal();
            }
        } else if (this.animationType == UiLayerAnimationType.PopInOut) {
            AnimationManager.Instance.Show(this.canvasGroup);

            this.overlayGroup.transform.localScale = Vector3.zero;

            AnimationManager.Instance.ScaleTo(this.overlayGroup, Vector3.one, 0.25f / this.animationSpeed, EasingType.EaseOutBack);

            if (this.backgroundGroup != null) {
                this.backgroundGroup.alpha = 0;

                AnimationManager.Instance.FadeIn(this.backgroundGroup, 0.2f);
            }

            if (this.revealer != null) {
                this.revealer.RevealAfter(0.25f / this.animationSpeed);
            }
        } else if (this.animationType == UiLayerAnimationType.DropInOut) {
            AnimationManager.Instance.Show(this.canvasGroup);

            this.overlayGroup.alpha = 0;
            this.overlayGroup.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, Screen.height, 0);

            AnimationManager.Instance.FadeIn(this.overlayGroup, 0.25f / this.animationSpeed);
            AnimationManager.Instance.MoveTo(this.overlayGroup, Vector3.zero, 0.25f / this.animationSpeed, EasingType.EaseOut);

            if (this.backgroundGroup != null) {
                this.backgroundGroup.alpha = 0;

                AnimationManager.Instance.FadeIn(this.backgroundGroup, 0.2f);
            }

            if (this.revealer != null) {
                this.revealer.RevealAfter(0.25f / this.animationSpeed);
            }
        } else if (this.animationType == UiLayerAnimationType.FadeInOut) {
            AnimationManager.Instance.FadeIn(this.canvasGroup, 0.25f / this.animationSpeed);

            if (this.revealer != null) {
                this.revealer.RevealAfter(0.25f / this.animationSpeed);
            }
        }

        if (!this.shouldSendStats) {
            return;
        }

        GameAnalytics.NewDesignEvent("UI:ChangeView:" + this.name);

        this.visibleAt = Time.time;
    }

    public virtual void Hide()
    {
        if (!this.IsVisible()) {
            return;
        }

        if (this.animationType == UiLayerAnimationType.None) {
            this.gameObject.SetActive(false);

            AnimationManager.Instance.Hide(this.canvasGroup);
        } else if (this.animationType == UiLayerAnimationType.PopInOut) {
            AnimationManager.Instance.ScaleTo(this.overlayGroup, Vector3.zero, 0.25f / this.animationSpeed, EasingType.EaseInBack);

            if (this.backgroundGroup != null) {
                AnimationManager.Instance.FireAfterDelay(delegate {
                    AnimationManager.Instance.FadeOut(this.backgroundGroup, 0.2f);
                }, 0.15f / this.animationSpeed);
            }

            AnimationManager.Instance.FireAfterDelay(delegate {
                this.gameObject.SetActive(false);

                AnimationManager.Instance.Hide(this.canvasGroup);
            }, 0.15f / this.animationSpeed + 0.2f);
        } else if (this.animationType == UiLayerAnimationType.DropInOut) {
            AnimationManager.Instance.FadeOut(this.overlayGroup, 0.25f / this.animationSpeed);
            AnimationManager.Instance.MoveTo(this.overlayGroup, new Vector3(0, -Screen.height, 0), 0.25f / this.animationSpeed, EasingType.EaseIn);

            if (this.backgroundGroup != null) {
                AnimationManager.Instance.FireAfterDelay(delegate {
                    AnimationManager.Instance.FadeOut(this.backgroundGroup, 0.2f);
                }, 0.15f / this.animationSpeed);
            }

            AnimationManager.Instance.FireAfterDelay(delegate {
                this.gameObject.SetActive(false);

                AnimationManager.Instance.Hide(this.canvasGroup);
            }, 0.15f / this.animationSpeed + 0.2f);
        } else if (this.animationType == UiLayerAnimationType.FadeInOut) {
            AnimationManager.Instance.FadeOut(this.canvasGroup, 0.25f / this.animationSpeed);
        }

        if (!this.shouldSendStats) {
            return;
        }

        GameAnalytics.NewDesignEvent("UI:ViewDuration:" + this.name, Time.time - this.visibleAt);
    }

    public virtual void Toggle()
    {
        if (this.IsVisible()) {
            this.Hide();
        } else {
            this.Show();
        }
    }
}
