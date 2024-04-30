using System.Collections.Generic;
using UnityEngine;

public class SkyAdventureCheckpointWindow : Window
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform rewardsList;
    List<GameObject> rewards;

    public void Init(List<JuicyTestReward> rewards)
    {
        this.rewards = new();
        rewards.ForEach(x => this.rewards.Add(x.Instantiate(rewardsList).gameObject));
        Show();
    }

    public override void Show()
    {
        UIAnimation.Appear(canvasGroup);
    }

    public override void Hide()
    {
        UIAnimation.Hide(canvasGroup, 0, () =>
        {
            rewards.ForEach(x => Destroy(x));
            rewards.Clear();
        });
        base.Hide();
    }
}