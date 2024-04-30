using UnityEngine;
using UnityEngine.UI;

public class JuicyTestX2Reward : RewardIcon
{
    [SerializeField] Image image;

    protected override void Awake()
    {
        image.sprite = GameData.Default.juicyTestData.types[GameData.Saves.JuicyTestCurType].icon;
        base.Awake();
    }
}