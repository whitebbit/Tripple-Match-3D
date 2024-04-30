using System.Collections.Generic;
using UnityEngine;

public class SkyAdventureCheckpointPopup : PopupWindow
{
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
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
        rewards.ForEach(x => Destroy(x));
        rewards.Clear();
    }
}