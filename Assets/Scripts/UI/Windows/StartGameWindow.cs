using System;
using DG.Tweening;
using Dreamteck;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StartGameWindow : PopupWindow
{
    [SerializeField] StartGameAbility[] abilities;
    [SerializeField] Transform juicyTestDropdown;
    [SerializeField] Transform crownRaceDropdown;
    [SerializeField] Transform eventDropdown;
    [SerializeField] TextMeshProUGUI juicyTestTimer, juicyTestCopyTimer;
    [SerializeField] Button eventButton;
    [SerializeField] LocalizeStringEvent eventName;
    [SerializeField] LocalizedString carRaceName, skyAdventureName;
    [SerializeField] Image eventIcon;
    [SerializeField] Sprite carRaceIcon, skyAdventureIcon;
    Tween juicyTestDropdownTween, crownRaceDropdownTween, eventDropdownTween;

    public event Action OnPlayEvent;

    void OnDisable()
    {
        juicyTestDropdown.gameObject.SetActive(false);
        crownRaceDropdown.gameObject.SetActive(false);
        eventDropdown.gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        if (GameData.Saves.JuicyTestCurType >= 0)
        {
            juicyTestDropdownTween.Kill(true);
            juicyTestDropdownTween = juicyTestDropdown.DOLocalMoveY(1200, 0.33f).From().OnStart(() => juicyTestDropdown.gameObject.SetActive(true));
        }

        if (CrownRace.Active)
        {
            crownRaceDropdownTween.Kill(true);
            crownRaceDropdownTween = crownRaceDropdown.DOLocalMoveY(0, 0.33f).From().SetDelay(0.2f).OnStart(() => crownRaceDropdown.gameObject.SetActive(true));
        }

        if (EventManager.CurEvent != BigEventType.None)
        {
            eventDropdownTween.Kill(true);
            eventDropdownTween = eventDropdown.DOLocalMoveY(-1410, 0.33f).SetDelay(0.4f).From().OnStart(() => eventDropdown.gameObject.SetActive(true));

            eventIcon.sprite = EventManager.CurEvent == BigEventType.SkyAdventure ? skyAdventureIcon : carRaceIcon;
            eventName.StringReference = EventManager.CurEvent == BigEventType.SkyAdventure ? skyAdventureName : carRaceName;
            eventName.RefreshString();

            eventButton.onClick.RemoveAllListeners();
            eventButton.onClick.AddListener(EventManager.CurEvent == BigEventType.SkyAdventure ? SkyAdventureWindow.Instance.AnimationShow : CarRaceWindow.Instance.AnimationShow);
            eventButton.onClick.AddListener(Hide);
        }
    }

    public override void Hide()
    {
        base.Hide();

        juicyTestDropdownTween.Kill(true);
        Vector3 juicyTestPos = juicyTestDropdown.localPosition;
        juicyTestDropdownTween = juicyTestDropdown.DOLocalMoveY(1200, 0.2f).OnComplete(() => juicyTestDropdown.localPosition = juicyTestPos);

        eventDropdownTween.Kill(true);
        Vector3 eventPos = eventDropdown.localPosition;
        eventDropdownTween = eventDropdown.DOLocalMoveY(-1410, 0.2f).OnComplete(() => eventDropdown.localPosition = eventPos);
    }

    public void Play()
    {
        Hide();
        abilities.ForEach(x => x.TryUse());
        OnPlayEvent?.Invoke();
    }

    void Update()
    {
        juicyTestTimer.text = juicyTestCopyTimer.text;
    }
}