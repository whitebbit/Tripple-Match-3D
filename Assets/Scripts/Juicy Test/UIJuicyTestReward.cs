using TMPro;
using UnityEngine;

public class UIJuicyTestReward : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lvlTxt;
    [SerializeField] Transform iconParent;
    [SerializeField] GameObject lockIcon;
    [SerializeField] GameObject checkMark;
    RewardIcon icon;

    public void Init(int lvl, Reward reward, bool unlock, bool received)
    {
        lvlTxt.text = lvl.ToString();
        if (received) checkMark.SetActive(true);
        else icon = reward.Instantiate(iconParent);
        lockIcon.SetActive(!unlock);
    }

    public void Unlock() => lockIcon.SetActive(false);

    public void Get()
    {
        lockIcon.SetActive(false);
        Destroy(icon.gameObject);
        checkMark.SetActive(true);
    }
}
