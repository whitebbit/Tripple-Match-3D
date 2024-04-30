using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRewardList : MonoBehaviour
{
    List<GameObject> rewards;
    Transform rewardsTransform;

    void Awake()
    {
        rewards = new();
        rewardsTransform = transform;
    }

    void OnEnable()
    {
        if (JuicyTestItemCounter.Instance.itemCount > 0) AddReward(GameData.Default.juicyTestData.types[GameData.Saves.JuicyTestCurType].icon, JuicyTestItemCounter.Instance.itemCount);
        List<Reward> levelRewards = new();
        for (int i = 0; i < Level.Instance.rewards.Count; i++)
        {
            Reward reward = levelRewards.Find(x => x.reward == Level.Instance.rewards[i].reward);
            if (reward == null) levelRewards.Add(Level.Instance.rewards[i]);
            else reward.count++;
        }
        for (int i = 0; i < levelRewards.Count; i++)
        {
            rewards.Add(levelRewards[i].Instantiate(rewardsTransform).gameObject);
            levelRewards[i].Get();
        }
    }

    void OnDisable()
    {
        rewards.ForEach(x => Destroy(x));
        rewards.Clear();
    }

    void AddReward(Sprite icon, int count)
    {
        UIIconCounter counter = Instantiate(GameData.Default.iconCounterUI, rewardsTransform);
        counter.Init(icon, count);
        rewards.Add(counter.gameObject);
    }
}