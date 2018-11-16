using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Object=UnityEngine.Object;

public enum EasingType
{
    None = 0,
    EaseIn = 1,
    EaseOut = 2,
    EaseInOut = 3,
    EaseInBack = 4,
    EaseOutBack = 5,
    EaseInOutBack = 6,
    EaseInBounce = 7,
    EaseOutBounce = 8,
    EaseInOutBounce = 9
}

public class AnimationManager : Singleton<AnimationManager>
{
    public Coroutine FireAfterDelay(Action callback, float delay)
    {
        return StartCoroutine(this.FireAfterDelayEnumerator(callback, delay));
    }

    private IEnumerator FireAfterDelayEnumerator(Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (callback != null) {
            callback();
        }
    }

    public Coroutine FireAfterDelayIfActive(MonoBehaviour target, Action callback, float delay)
    {
        return this.FireAfterDelayIfActive(target.gameObject, callback, delay);
    }

    public Coroutine FireAfterDelayIfActive(Transform target, Action callback, float delay)
    {
        return this.FireAfterDelayIfActive(target.gameObject, callback, delay);
    }

    public Coroutine FireAfterDelayIfActive(GameObject target, Action callback, float delay)
    {
        return StartCoroutine(this.FireAfterDelayIfActiveEnumerator(target, callback, delay));
    }

    private IEnumerator FireAfterDelayIfActiveEnumerator(GameObject target, Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target.activeInHierarchy) {
            if (callback != null) {
                callback();
            }
        } else {
            Logger.MessageFormat("Ignoring delayed fire because `{0}` is inactive...", target.name);
        }
    }

    public void Show(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void Hide(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
    }

    public Coroutine FadeIn(Renderer renderer, float duration, Action callback = null)
    {
        return this.FadeIn(renderer, duration, EasingType.None, callback);
    }

    public Coroutine FadeIn(Renderer renderer, float duration, EasingType easingType, Action callback = null)
    {
        return StartCoroutine(this.FadeEnumerator(renderer, duration, 1f, easingType, callback));
    }

    public Coroutine FadeOut(Renderer renderer, float duration, Action callback = null)
    {
        return this.FadeOut(renderer, duration, EasingType.None, callback);
    }

    public Coroutine FadeOut(Renderer renderer, float duration, EasingType easingType, Action callback = null)
    {
        return StartCoroutine(this.FadeEnumerator(renderer, duration, 0, easingType, callback));
    }

    private IEnumerator FadeEnumerator(Renderer renderer, float duration, float targetAlpha, EasingType easingType, Action callback)
    {
        Color startColor = renderer.material.color;
        Color targetColor = renderer.material.color;

        targetColor.a = targetAlpha;

        for (float timeElapsed = 0; timeElapsed < 1f; timeElapsed += Time.deltaTime / duration) {
            if (this.IsComponentNull(renderer)) {
                break;
            }

            renderer.material.color = Color.Lerp(startColor, targetColor, this.GetEasedRatio(timeElapsed, easingType));

            yield return null;
        }

        if (callback != null) {
            callback();
        }

        if (this.IsComponentNull(renderer)) {
            yield break;
        }

        renderer.material.color = targetColor;
    }

    public Coroutine FadeIn(CanvasGroup canvasGroup, float duration, Action callback = null)
    {
        return this.FadeIn(canvasGroup, duration, EasingType.None, callback);
    }

    public Coroutine FadeIn(CanvasGroup canvasGroup, float duration, EasingType easingType, Action callback = null)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        return StartCoroutine(this.FadeEnumerator(canvasGroup, duration, 1f, easingType, callback));
    }

    public Coroutine FadeOut(CanvasGroup canvasGroup, float duration, Action callback = null)
    {
        return this.FadeOut(canvasGroup, duration, EasingType.None, callback);
    }

    public Coroutine FadeOut(CanvasGroup canvasGroup, float duration, EasingType easingType, Action callback = null)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        return StartCoroutine(this.FadeEnumerator(canvasGroup, duration, 0, easingType, callback));
    }

    private IEnumerator FadeEnumerator(CanvasGroup canvasGroup, float duration, float targetAlpha, EasingType easingType, Action callback)
    {
        float startAlpha = canvasGroup.alpha;

        if (targetAlpha == 1f) {
            canvasGroup.gameObject.SetActive(true);
        }

        for (float timeElapsed = 0; timeElapsed < 1f; timeElapsed += Time.deltaTime / duration) {
            if (this.IsComponentNull(canvasGroup)) {
                break;
            }

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, this.GetEasedRatio(timeElapsed, easingType));

            yield return null;
        }

        if (callback != null) {
            callback();
        }

        if (this.IsComponentNull(canvasGroup)) {
            yield break;
        }

        if (targetAlpha == 0) {
            canvasGroup.gameObject.SetActive(false);
        }

        canvasGroup.alpha = targetAlpha;
    }

    public Coroutine MoveTo(CanvasGroup canvasGroup, Vector3 position, float duration, Action callback = null)
    {
        return this.MoveTo(canvasGroup.GetComponent<RectTransform>(), position, duration, callback);
    }

    public Coroutine MoveTo(CanvasGroup canvasGroup, Vector3 position, float duration, EasingType easingType, Action callback = null)
    {
        return this.MoveTo(canvasGroup.GetComponent<RectTransform>(), position, duration, easingType, callback);
    }

    public Coroutine MoveTo(RectTransform rectTransform, Vector3 position, float duration, Action callback = null)
    {
        return this.MoveTo(rectTransform, position, duration, EasingType.None, callback);
    }

    public Coroutine MoveTo(RectTransform rectTransform, Vector3 position, float duration, EasingType easingType, Action callback = null)
    {
        return StartCoroutine(this.MoveEnumerator(rectTransform, duration, position, easingType, callback));
    }

    private IEnumerator MoveEnumerator(RectTransform rectTransform, float duration, Vector3 targetPosition, EasingType easingType, Action callback)
    {
        Vector3 startPosition = rectTransform.anchoredPosition;

        for (float timeElapsed = 0; timeElapsed < 1f; timeElapsed += Time.deltaTime / duration) {
            if (this.IsComponentNull(rectTransform)) {
                break;
            }

            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, this.GetEasedRatio(timeElapsed, easingType));

            yield return null;
        }

        if (callback != null) {
            callback();
        }

        if (this.IsComponentNull(rectTransform)) {
            yield break;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    public Coroutine ScaleTo(CanvasGroup canvasGroup, Vector3 position, float duration, Action callback = null)
    {
        return this.ScaleTo(canvasGroup.GetComponent<RectTransform>(), position, duration, callback);
    }

    public Coroutine ScaleTo(CanvasGroup canvasGroup, Vector3 position, float duration, EasingType easingType, Action callback = null)
    {
        return this.ScaleTo(canvasGroup.GetComponent<RectTransform>(), position, duration, easingType, callback);
    }

    public Coroutine ScaleTo(RectTransform rectTransform, Vector3 position, float duration, Action callback = null)
    {
        return this.ScaleTo(rectTransform, position, duration, EasingType.None, callback);
    }

    public Coroutine ScaleTo(RectTransform rectTransform, Vector3 position, float duration, EasingType easingType, Action callback = null)
    {
        return StartCoroutine(this.ScaleEnumerator(rectTransform, duration, position, easingType, callback));
    }

    private IEnumerator ScaleEnumerator(RectTransform rectTransform, float duration, Vector3 targetScale, EasingType easingType, Action callback)
    {
        Vector3 startScale = rectTransform.localScale;

        for (float timeElapsed = 0; timeElapsed < 1f; timeElapsed += Time.deltaTime / duration) {
            if (this.IsComponentNull(rectTransform)) {
                break;
            }

            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, this.GetEasedRatio(timeElapsed, easingType));

            yield return null;
        }

        if (callback != null) {
            callback();
        }

        if (this.IsComponentNull(rectTransform)) {
            yield break;
        }

        rectTransform.localScale = targetScale;
    }

    public bool IsComponentNull(Object component)
    {
        if (component == null) {
            return true;
        }

        if (component.Equals(null)) {
            return true;
        }

        return false;
    }

    public float GetEasedRatio(float timeElapsed, EasingType easingType)
    {
        switch (easingType) {
            case EasingType.EaseIn:
                return timeElapsed * timeElapsed;

            case EasingType.EaseOut:
                return timeElapsed * (2f - timeElapsed);

            case EasingType.EaseInOut:
                return timeElapsed < 0.5f ? 2 * timeElapsed * timeElapsed : -1f + timeElapsed * (4f - 2f * timeElapsed);

            case EasingType.EaseInBack:
                return timeElapsed * timeElapsed * (2.70158f * timeElapsed - 1.70158f);

            case EasingType.EaseOutBack:
                return 1f + (timeElapsed - 1f) * (timeElapsed - 1f) * (2.70158f * (timeElapsed - 1f) + 1.70158f);

            case EasingType.EaseInOutBack:
                return timeElapsed < 0.5f ? timeElapsed * timeElapsed * (7f * timeElapsed - 2.5f) * 2f : 1f + (timeElapsed - 1f) * (timeElapsed - 1f) * 2f * (7f * timeElapsed + 2.5f);

            case EasingType.EaseInBounce:
                return Mathf.Pow(2f, 6f * (timeElapsed - 1f)) * Mathf.Abs(Mathf.Sin(timeElapsed * 3.5f));

            case EasingType.EaseOutBounce:
                return 1f - Mathf.Pow(2f, -6f * timeElapsed) * Mathf.Abs(Mathf.Cos(timeElapsed * 3.5f));

            case EasingType.EaseInOutBounce:
                return timeElapsed < 0.5f ? 8f * Mathf.Pow(2f, 8f * (timeElapsed - 1)) * Mathf.Abs(Mathf.Sin(timeElapsed * 7f)) : 1f - 8f * Mathf.Pow(2f, -8f * timeElapsed) * Mathf.Abs(Mathf.Sin(timeElapsed * 7f));
        }

        return timeElapsed;
    }
}
