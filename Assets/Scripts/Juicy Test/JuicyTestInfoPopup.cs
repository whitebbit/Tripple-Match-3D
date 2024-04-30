using UnityEngine;
using UnityEngine.UI;

public class JuicyTestInfoPopup : PopupWindow
{
    [SerializeField] Image eventIcon;

    void OnEnable()
    {
        eventIcon.sprite = GameData.Default.juicyTestData.types[Mathf.Max(GameData.Saves.JuicyTestCurType, 0)].eventIcon;
    }
}