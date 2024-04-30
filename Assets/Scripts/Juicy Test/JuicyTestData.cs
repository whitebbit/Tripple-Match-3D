using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Data/Juicy Test Data")]
public class JuicyTestData : ScriptableObject
{
    public JuicyTestReward[] rewards;

    [Serializable]
    public class Type
    {
        [ItemType] public int item;
        public LocalizedString name, discription;
        public Sprite icon;
        public Sprite eventIcon;
    }
    public Type[] types;
}

[Serializable]
public class JuicyTestReward : Reward
{
    public int targetScore;
}