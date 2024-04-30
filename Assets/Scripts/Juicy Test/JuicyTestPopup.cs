using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuicyTestPopup : PopupWindow
{
    JuicyTestData data;

    [SerializeField] Image eventIcon;

    [SerializeField] Slider slider;
    [SerializeField] Image itemIcon;
    [SerializeField] Transform rewardTransform;
    [SerializeField] TextMeshProUGUI sliderCounter;
    [SerializeField] TextMeshProUGUI timer, copyTimer;
    GameObject curReward;

    [SerializeField] ScrollRect rewardsList;
    [SerializeField] UIJuicyTestReward reward;
    List<UIJuicyTestReward> rewards;
    int oldLvl;

    public void Init()
    {
        data = GameData.Default.juicyTestData;

        oldLvl = GameData.Saves.JuicyTestCurLvl;
        if (rewards == null) rewards = new();
        else
        {
            rewards.ForEach(x => Destroy(x.gameObject));
            rewards.Clear();
        }
        for (int i = 0; i < data.rewards.Length; i++)
        {
            UIJuicyTestReward reward = Instantiate(this.reward, rewardsList.content);
            reward.Init(i + 1, data.rewards[i], GameData.Saves.JuicyTestCurLvl >= i, GameData.Saves.JuicyTestCurLvl > i);
            rewards.Add(reward);
            reward.transform.SetAsFirstSibling();
        }

        eventIcon.sprite = data.types[GameData.Saves.JuicyTestCurType].eventIcon;
    }

    void OnEnable()
    {
        if (GameData.Saves.JuicyTestCurLvl < 30)
        {
            var reward = data.rewards[GameData.Saves.JuicyTestCurLvl];
            int value = GameData.Saves.JuicyTestCurValue;
            int targetScore = reward.targetScore;
            slider.value = (float)value / targetScore;
            sliderCounter.text = $"{value}/{targetScore}";
            itemIcon.sprite = data.types[GameData.Saves.JuicyTestCurType].icon;
            if (curReward) Destroy(curReward);
            curReward = reward.Instantiate(rewardTransform).gameObject;
        }
        else
        {
            if (curReward) Destroy(curReward);
            slider.value = 1;
            sliderCounter.text = "";
        }

        Update();

        for (int i = oldLvl; i < GameData.Saves.JuicyTestCurLvl; i++) rewards[i].Get();
        if (GameData.Saves.JuicyTestCurLvl < 30) rewards[GameData.Saves.JuicyTestCurLvl].Unlock();
        oldLvl = GameData.Saves.JuicyTestCurLvl;

        rewardsList.verticalNormalizedPosition = -0.1f + oldLvl / 25f;
    }

    void Update()
    {
        timer.text = copyTimer.text;
    }
}
