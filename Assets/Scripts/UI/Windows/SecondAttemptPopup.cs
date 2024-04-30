using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondAttemptPopup : PopupWindow
{
    enum SecondAttemptType
    {
        CleanUp, AddTime
    }

    [SerializeField] SecondAttemptType type;
    [SerializeField] int[] prices;

    [SerializeField] Transform dropdownPanel;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] CanvasGroup givenRewards, lostRewards;
    [SerializeField] GameObject additionalGivenReward;
    [SerializeField] UIIconCounter thunderCounter;
    [SerializeField] ScrollRect lostRewardsScrollRect;
    [SerializeField] GameObject carRaceEventReward, skyAdventureEventReward, crownRaceReward;
    [SerializeField] GameObject heart;
    bool showLostRewards;
    int attempt;
    Sequence lostRewardsScrollSequence;
    Tween dropdownTween;

    void Awake()
    {
        GameManager.OnRestartEvent += () => attempt = 0;
    }

    void OnEnable()
    {
        showLostRewards = false;
        givenRewards.gameObject.SetActive(true);
        lostRewards.gameObject.SetActive(false);
        price.text = prices[Mathf.Min(attempt, prices.Length - 1)].ToString();
        if (attempt >= 1)
        {
            additionalGivenReward.SetActive(true);
            thunderCounter.SetCount(Mathf.Min(attempt, 2) * 3);
        }

        dropdownTween.Kill(true);
        dropdownTween = dropdownPanel.DOLocalMoveY(0, 0.33f).From().SetDelay(0.2f).OnStart(() => dropdownPanel.gameObject.SetActive(true));
    }

    public override void Hide()
    {
        if (!showLostRewards)
        {
            showLostRewards = true;
            UIAnimation.Fade(givenRewards);
            UIAnimation.Appear(lostRewards);
            heart.SetActive(!GameData.InfinityHP);

            int lostRewardsPlaneCount = 1;

            carRaceEventReward.SetActive(GameData.Saves.CarRaceIsStarted);
            if (GameData.Saves.CarRaceIsStarted) lostRewardsPlaneCount++;
            skyAdventureEventReward.SetActive(GameData.Saves.SkyAdventureLvl >= 0);
            if (GameData.Saves.SkyAdventureLvl >= 0) lostRewardsPlaneCount++;
            crownRaceReward.SetActive(GameData.Saves.CrownRaceScore >= 1);
            if (GameData.Saves.CrownRaceScore >= 1) lostRewardsPlaneCount++;

            lostRewardsScrollSequence.Kill();
            lostRewardsScrollRect.DONormalizedPos(Vector2.one, 0.05f).OnComplete(() =>
            {
                lostRewardsScrollSequence = DOTween.Sequence().SetLoops(-1, LoopType.Restart);
                for (int i = 0; i < lostRewardsPlaneCount; i++)
                {
                    float pos = (float)i / (lostRewardsPlaneCount - 1);
                    lostRewardsScrollSequence.Append(lostRewardsScrollRect.DONormalizedPos(Vector2.right * pos, 0.33f)).AppendInterval(1.0f);
                }
            });
            return;
        }

        base.Hide();
        lostRewardsScrollSequence.Kill();
        Level.Instance.InvokeGameFail();
    }

    public void Buy()
    {
        if (!GameData.Default.TryPayCoins(prices[Mathf.Min(attempt, prices.Length - 1)]))
        {
            UIManager.Instance.ShowShopPopup();
            return;
        }

        switch (type)
        {
            case SecondAttemptType.CleanUp:
                {
                    for (int i = 0; i < 7; i++) Inventory.Instance.TryRemoveItem();
                }
                break;
            case SecondAttemptType.AddTime:
                {
                    Level.Instance.time += 60;
                    UIManager.Instance.inGameWindow.addTimeParticles.Play();
                }
                break;
        }
        Level.Instance.UseThunder(Mathf.Min(attempt, 2) * 3);
        GameContinue();
    }

    public void WatchAD() => ADManager.ShowRewardVideo(GetADReward);
    void GetADReward()
    {
        switch (type)
        {
            case SecondAttemptType.CleanUp:
                {
                    for (int i = 0; i < 7; i++) Inventory.Instance.TryRemoveItem();
                }
                break;
            case SecondAttemptType.AddTime:
                {
                    Level.Instance.time += 45;
                    UIManager.Instance.inGameWindow.addTimeParticles.Play();
                }
                break;
        }
        GameContinue();
    }

    void GameContinue()
    {
        attempt++;
        SoundHolder.Default.PlayFromSoundPack("Buy");
        Level.Instance.GameContinue();
        base.Hide();
    }
}