using UnityEngine;

[CreateAssetMenu(menuName = "Data/In Game Purchased")]
public class InGamePurchasedData : ScriptableObject
{
    [System.Serializable]
    public class Rewards
    {
        public Reward[] rewards;
    }

    public Rewards[] rewards;
    public Reward[] this[int i] => rewards[i].rewards;
}
