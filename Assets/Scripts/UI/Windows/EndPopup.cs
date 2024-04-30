using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class EndPopup : Window
{
    [SerializeField] Image background;
    [SerializeField] GameObject frame;

    public static event Action ReturnToMenuEvent;
    public static event Action RestartLvlEvent;

    void OnEnable()
    {
        GameManager.OnRestartEvent += Hide;
    }

    void OnDisable()
    {
        GameManager.OnRestartEvent -= Hide;
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        UIAnimation.Appear(background);
        UIAnimation.Show(frame);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        base.Hide();
    }

    public void ReturnToMenu() => ReturnToMenuEvent?.Invoke();
    public void RestartLvl() => RestartLvlEvent?.Invoke();
}
