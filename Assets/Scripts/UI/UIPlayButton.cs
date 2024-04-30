using UnityEngine;
using UnityEngine.UI;

public class UIPlayButton : MonoBehaviour
{
    [SerializeField] GameObject[] flags;
    int curFlag;

    [SerializeField] Image crownIcon;

    void Awake()
    {
        GameManager.OnRestartEvent += UpdateButton;
        if (GameManager.GameEnabled) UpdateButton();
        else GameManager.OnInitEvent += UpdateButton;
    }

    void UpdateButton()
    {
        flags[curFlag].SetActive(false);
        if (Level.Instance.difficulty != LevelDifficult.None)
        {
            curFlag = (int)Level.Instance.difficulty - 1;
            flags[curFlag].SetActive(true);
        }

        if (CrownRace.Active)
        {
            int lvl = Mathf.Min(GameData.Saves.CrownRaceScore - 1, 2);
            crownIcon.gameObject.SetActive(lvl >= 0);
            if (lvl >= 0) crownIcon.sprite = GameData.Default.crownRaceIcons[lvl];
        }
        else crownIcon.gameObject.SetActive(false);
    }
}
