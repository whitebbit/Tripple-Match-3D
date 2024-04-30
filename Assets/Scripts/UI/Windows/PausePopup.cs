using System;
using UnityEngine;
using UnityEngine.UI;

public class PausePopup : Window
{
    [SerializeField] Image background;
    [SerializeField] GameObject frame;

    public event Action ResumeGameEvent;

    void OnEnable()
    {
        GameManager.OnRestartEvent += InstantlyHide;
    }

    void OnDisable()
    {
        GameManager.OnRestartEvent -= InstantlyHide;
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        UIAnimation.Appear(background);
        UIAnimation.Show(frame);
    }

    void InstantlyHide() => gameObject.SetActive(false);
    public override void Hide()
    {
        UIAnimation.Fade(background, 0, 0.33f, () => gameObject.SetActive(false));
        UIAnimation.Hide(frame);
        base.Hide();
    }

    public void ResumeGame() => ResumeGameEvent?.Invoke();

    public void ReturnToMainMenu()
    {
        ResumeGame();
        Level.Instance.GameFail(FailType.Exit);
    }
}
