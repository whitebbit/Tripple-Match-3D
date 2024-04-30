using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class SkyAdventureWinWindow : Window
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform rewardsList;
    [SerializeField] Transform playersList;
    [SerializeField] LocalizeStringEvent playersWin;
    List<GameObject> rewards, players;

    public void Init(List<JuicyTestReward> rewards, List<SkyAdventurePlayer> players)
    {
        this.rewards = new();
        this.players = new();
        rewards.ForEach(x => this.rewards.Add(x.Instantiate(rewardsList).gameObject));
        players.ForEach(x =>
        {
            Destroy(x.GetComponent<Billboard>());
            RectTransform transform = x.gameObject.AddComponent<RectTransform>();
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -1);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 90;
            GameObject player = Instantiate(x, playersList).gameObject;
            this.players.Add(player);
        });
        playersWin.StringReference.Arguments = new object[] { players.Count };
        playersWin.RefreshString();
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
            players.ForEach(x => Destroy(x));
            players.Clear();
        });
        base.Hide();
    }
}