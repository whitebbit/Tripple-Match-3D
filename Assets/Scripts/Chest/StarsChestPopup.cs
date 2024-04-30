using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarsChestPopup : PopupWindow
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI counter;

    public override void Show()
    {
        slider.value = (float)GameData.Saves.Stars / GameData.Default.chestNeedStars;
        counter.text = $"{GameData.Saves.Stars}/{GameData.Default.chestNeedStars}";
        base.Show();
    }
}
