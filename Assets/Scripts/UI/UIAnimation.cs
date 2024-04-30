using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIAnimation
{
    static Dictionary<int, Tween> tweens;
    static UIAnimation()
    {
        tweens = new();
    }

    static void TrySetButton(GameObject element, bool interactable)
    {
        if (element.TryGetComponent(out Button button)) button.interactable = interactable;
    }

    public static Tween Show(Component element, float delay = 0, Action action = null) => Show(element.gameObject, delay, action);
    public static Tween Show(GameObject element, float delay = 0, Action action = null)
    {
        TrySetButton(element, true);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        element.SetActive(false);

        tween = transform.DOScale(0, 0.33f).From().SetDelay(delay).SetEase(Ease.OutBack).SetUpdate(true);
        tweens[hash] = tween;
        action = (() => tweens.Remove(hash)) + action;
        tween.OnStart(() => element.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Hide(Component element, float delay = 0, Action action = null) => Hide(element.gameObject, delay, action);
    public static Tween Hide(GameObject element, float delay = 0, Action action = null)
    {
        TrySetButton(element, false);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        Vector3 scale = transform.localScale;

        tween = transform.DOScale(0, 0.33f).SetDelay(delay).SetEase(Ease.InBack).SetUpdate(true);
        tweens[hash] = tween;
        action = () => { element.SetActive(false); transform.localScale = scale; tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Appear(Image element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, true);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        gameObject.SetActive(false);

        tween = element.DOFade(0, duration).From().SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = (() => tweens.Remove(hash)) + action;
        tween.OnStart(() => gameObject.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Fade(Image element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, false);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        float alpha = element.color.a;

        tween = element.DOFade(0, duration).SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = () => { gameObject.SetActive(false); element.color = new Color(element.color.r, element.color.g, element.color.b, alpha); tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Appear(SpriteRenderer element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, true);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        gameObject.SetActive(false);

        tween = element.DOFade(0, duration).From().SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = (() => tweens.Remove(hash)) + action;
        tween.OnStart(() => gameObject.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Fade(SpriteRenderer element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, false);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        float alpha = element.color.a;

        tween = element.DOFade(0, duration).SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = () => { gameObject.SetActive(false); element.color = new Color(element.color.r, element.color.g, element.color.b, alpha); tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Appear(TMP_Text element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, true);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        gameObject.SetActive(false);

        tween = element.DOFade(0, duration).From().SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = (() => tweens.Remove(hash)) + action;
        tween.OnStart(() => gameObject.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Fade(TMP_Text element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, false);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        float alpha = element.color.a;

        tween = element.DOFade(0, duration).SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = () => { gameObject.SetActive(false); element.color = new Color(element.color.r, element.color.g, element.color.b, alpha); tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Appear(CanvasGroup element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, true);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        gameObject.SetActive(false);

        tween = element.DOFade(0, duration).From().SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = (() => tweens.Remove(hash)) + action;
        tween.OnStart(() => gameObject.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween Fade(CanvasGroup element, float delay = 0, float duration = 0.33f, Action action = null)
    {
        GameObject gameObject = element.gameObject;

        TrySetButton(gameObject, false);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        float alpha = element.alpha;

        tween = element.DOFade(0, duration).SetDelay(delay).SetUpdate(true);
        tweens[hash] = tween;
        action = () => { gameObject.SetActive(false); element.alpha = alpha; tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween SlideOut(Component element, Vector2 side = default, float delay = 0, Action action = null) => SlideOut(element.gameObject, side, delay, action);
    public static Tween SlideOut(GameObject element, Vector2 side = default, float delay = 0, Action action = null)
    {
        TrySetButton(element, true);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        element.SetActive(false);

        if (side == default) side = Vector2.right;
        if (side.x == 0) tween = transform.DOLocalMoveY(Screen.height * 2 * side.y, 0.4f).From();
        else if (side.y == 0) tween = transform.DOLocalMoveX(Screen.width * 2 * side.x, 0.4f).From();
        else tween = transform.DOLocalMove(new Vector2(Screen.width * 2 * side.x, Screen.height * 2 * side.y), 0.4f).From();
        tween.SetDelay(delay).SetUpdate(true);

        tweens[hash] = tween;
        action = () => { tweens.Remove(hash); } + action;
        tween.OnStart(() => element.SetActive(true)).OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween SlideIn(Component element, Vector2 side = default, float delay = 0, Action action = null) => SlideIn(element.gameObject, side, delay, action);
    public static Tween SlideIn(GameObject element, Vector2 side = default, float delay = 0, Action action = null)
    {
        TrySetButton(element, false);

        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        Vector3 pos = transform.localPosition;

        if (side == default) side = Vector2.right;
        if (side.x == 0) tween = transform.DOLocalMoveY(Screen.height * 2 * side.y, 0.4f);
        else if (side.y == 0) tween = transform.DOLocalMoveX(Screen.width * 2 * side.x, 0.4f);
        else tween = transform.DOLocalMove(new Vector2(Screen.width * 2 * side.x, Screen.height * 2 * side.y), 0.4f);
        tween.SetDelay(delay).SetUpdate(true);

        tweens[hash] = tween;
        action = () => { element.SetActive(false); transform.localPosition = pos; tweens.Remove(hash); } + action;
        tween.OnComplete(() => action.Invoke());
        return tween;
    }

    public static Tween ScaleAnimation(Component element, Vector3 scale = new Vector3(), float delay = 0) => ScaleAnimation(element.gameObject, scale, delay);
    public static Tween ScaleAnimation(GameObject element, Vector3 scale = new Vector3(), float delay = 0)
    {
        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;
        if (scale == new Vector3()) scale = Vector3.one * 0.15f;
        Vector3 startScale = transform.localScale;

        tween = transform.DOPunchScale(scale, 0.66f, 0, 0).SetDelay(delay).SetUpdate(true).SetAutoKill(false).OnComplete(() => tween.Restart()).OnKill(() => { transform.localScale = startScale; tweens.Remove(hash); });
        tweens[hash] = tween;
        return tween;
    }

    public static Tween ShakeAnimation(Component element, float duration = 1.0f, float strength = 9.0f, float delay = 0) => ShakeAnimation(element.gameObject, duration, strength, delay);
    public static Tween ShakeAnimation(GameObject element, float duration = 1.0f, float strength = 9.0f, float delay = 0)
    {
        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);

        Transform transform = element.transform;

        tween = transform.DOShakeRotation(duration, strength).SetDelay(delay).SetUpdate(true).SetAutoKill(false).OnComplete(() => tween.Restart()).OnKill(() => tweens.Remove(hash));
        tweens[hash] = tween;
        return tween;
    }

    public static void StopAnimation(GameObject element)
    {
        int hash = element.GetHashCode();
        if (tweens.TryGetValue(hash, out Tween tween)) tween.Kill(true);
    }
}
