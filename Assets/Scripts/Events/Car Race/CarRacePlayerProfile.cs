using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class CarRacePlayerProfile : MonoBehaviour
{
    [SerializeField] Image frame, icon;
    [SerializeField] LocalizeStringEvent nickName;

    public void Init(Sprite frame, Sprite icon, LocalizedString name)
    {
        this.frame.sprite = frame;
        this.icon.sprite = icon;
        nickName.StringReference = name;
        nickName.RefreshString();
    }
}
