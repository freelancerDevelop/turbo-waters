using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UiWidget : MonoBehaviour
{
    protected CanvasGroup canvasGroup;

    public virtual void Start()
    {
        this.canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public virtual bool IsVisible()
    {
        return this.gameObject.activeSelf;
    }

    public virtual void Show()
    {
        this.gameObject.SetActive(true);

        this.canvasGroup.alpha = 1f;
        this.canvasGroup.interactable = true;
        this.canvasGroup.blocksRaycasts = true;
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);

        this.canvasGroup.alpha = 0;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;
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
