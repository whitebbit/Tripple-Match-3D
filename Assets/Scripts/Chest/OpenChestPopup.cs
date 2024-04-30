using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OpenChestPopup : Window
{
    #region Singletone
    private static OpenChestPopup _instance;
    public static OpenChestPopup Instance { get => _instance; }
    public OpenChestPopup() => _instance = this;
    #endregion

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Chest chest;
    [SerializeField] Transform rewardsTransform;
    [SerializeField] GameObject tapToOpenTxt, tapToContinueTxt;
    List<GameObject> rewards;
    Reward[] curRewards;

    event Action OnOpenEvent, OnHideEvent;

    void Awake() => rewards = new();

    public void Show(Reward[] rewards, Action onOpen, Action onHide = null)
    {
        curRewards = rewards;
        OnOpenEvent = onOpen;
        OnHideEvent = onHide;
        Show();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        UIAnimation.Appear(canvasGroup, 0, 0.33f, () => chest.CurState = Chest.State.Fall);
        UIAnimation.Show(tapToOpenTxt, 0.33f);
    }

    public override void Hide()
    {
        rewards.ForEach(x => UIAnimation.Hide(x, 0, () => Destroy(x)));
        rewards.Clear();

        UIAnimation.Hide(chest, 0, () => chest.gameObject.SetActive(true));
        UIAnimation.Fade(canvasGroup, 0, 0.33f, () =>
        {
            chest.CurState = Chest.State.None;
            gameObject.SetActive(false);
            tapToContinueTxt.SetActive(false);
            base.Hide();
            OnHideEvent?.Invoke();
            OnHideEvent = null;
        });
    }

    public void Click()
    {
        switch (chest.CurState)
        {
            case Chest.State.Idle:
                {
                    chest.CurState = Chest.State.Open;
                    UIAnimation.Hide(tapToOpenTxt, 0, () => UIAnimation.Show(tapToContinueTxt));
                    Open();
                }
                break;
            case Chest.State.Reduce:
                {
                    Hide();
                }
                break;
        }
    }

    void Open()
    {
        for (int i = 0; i < curRewards.Length; i++) GiveReward(curRewards[i], i);
        SoundHolder.Default.PlayFromSoundPack("Open Chest");

        OnOpenEvent?.Invoke();
        OnOpenEvent = null;
    }

    void GiveReward(Reward reward, int order)
    {
        reward.Get();
        RewardIcon rew = reward.Instantiate(rewardsTransform);
        rew.thisTransform.DOLocalJump(rewardsTransform.localPosition + new Vector3(-250 + (float)order / (curRewards.Length - 1) * 500, 250, 0), 1, 1, 0.33f).SetDelay(0.1f + order * 0.05f);
        rewards.Add(rew.gameObject);
    }
}