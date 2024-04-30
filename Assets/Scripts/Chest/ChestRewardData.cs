using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chest Reward Data")]
public class ChestRewardData : ScriptableObject
{
    [System.Serializable]
    public class Rewards
    {
        public Reward[] rewards;
    }

    public Rewards[] rewards;
    public Reward[] this[int i] => rewards[i].rewards;
}