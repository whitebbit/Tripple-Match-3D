using UnityEngine;
using UnityEngine.UI;

public class UICrownRaceRewardList : MonoBehaviour
{
    [SerializeField] UIIconCounter thunderReward, addTimeReward;
    [SerializeField] Image crownIcon;

    void OnEnable()
    {
        int lvl = Mathf.Clamp(GameData.Saves.CrownRaceScore - 1, 0, 2);
        crownIcon.sprite = GameData.Default.crownRaceIcons[lvl];
        thunderReward.SetCount(lvl + 1);
        addTimeReward.SetCount(lvl + 1);
    }
}
