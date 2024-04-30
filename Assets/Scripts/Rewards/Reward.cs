using System;
using UnityEngine;

[Serializable]
public class Reward
{
    public RewardType reward;
    public int count;

    public RewardIcon icon => GameData.Default.rewardIcons[(int)reward];
    
    public RewardIcon Instantiate(Transform parent)
    {
        RewardIcon counter = GameObject.Instantiate(icon, parent);
        counter.Init(count);
        return counter;
    }

    public void Get()
    {
        switch (reward)
        {
            case RewardType.InfinityHearts:
                {
                    GameData.Default.AddInfinityHP(count);
                }
                break;
            case RewardType.InfinityThunder:
                {
                    AbilityThunder.AddInfinity(count);
                }
                break;
            case RewardType.InfinityAddTime:
                {
                    AbilityAddTime.AddInfinity(count);
                }
                break;
            case RewardType.X2:
                {
                    UIJuicyTestSlider.Instance.AddX2(count);
                }
                break;
            case RewardType.Coins:
                {
                    if (GameManager.Instance.State == GameState.StartMenu) UIManager.GetReward(Vector2.zero, count);
                    else GameData.Default.AddCoins(count);
                }
                break;
            default:
                {
                    GameData.Saves.AbilityCount[(int)reward - 5] += count;
                    GameData.SaveProgress();
                }
                break;
        }
    }
}

public enum RewardType
{
    InfinityHearts, InfinityThunder, InfinityAddTime, X2, Coins, Hint, Return, Raise, FreezeTime, Thunder, AddTime
}