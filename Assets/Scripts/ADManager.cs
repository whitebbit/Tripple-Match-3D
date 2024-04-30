using System;
using UnityEngine;
using YG;

public class ADManager : MonoBehaviour
{
    void OnEnable()
    {
        YandexGame.RewardVideoEvent += GetReward;
        GameManager.OnRestartEvent += ShowInter;
    }

    void OnDisable()
    {
        YandexGame.RewardVideoEvent -= GetReward;
        GameManager.OnRestartEvent -= ShowInter;
    }

    public static void ShowInter()
    {
        if (GameData.Saves.NoAds) return;
        YandexGame.FullscreenShow();
    }

    static event Action getRewardEvent;
    public static void ShowRewardVideo(Action action = null)
    {
        getRewardEvent = action;
        YandexGame.RewVideoShow(1);
    }

    static void GetReward(int id)
    {
        if (id == 1) getRewardEvent?.Invoke();
    }
}
