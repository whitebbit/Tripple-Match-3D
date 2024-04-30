using UnityEngine;
using UnityEngine.UI;

public class CrownRaceSlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image[] lvlsImage;
    int curLvl;

    void OnEnable()
    {
        slider.value = GameData.Saves.CrownRaceScore / 3f;
        int lvl = Mathf.Min(GameData.Saves.CrownRaceScore - 1, 2);
        lvlsImage[curLvl].enabled = false;
        if (lvl >= 0)
        {
            curLvl = lvl;
            lvlsImage[curLvl].enabled = true;
        }
    }
}