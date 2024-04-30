using UnityEngine;
using UnityEngine.Localization.Components;

public class LvlChestPopup : PopupWindow
{
    [SerializeField] LocalizeStringEvent stringEvent;

    void OnEnable() => stringEvent.StringReference.Arguments = new object[] { GameData.Default.chestNeedLvl * (GameData.Saves.LvlChestOpen + 1) };
}
