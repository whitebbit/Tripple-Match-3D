using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StartWindow : Window
{
    public CanvasGroup canvasGroup;
    [SerializeField] StartGameWindow startGamePanel;
    [SerializeField] GameObject juicyTestInfo;
    [SerializeField] LocalizeStringEvent juicyTestInfoTxt;
    [SerializeField] UIJuicyTestSlider juicyTestSlider;
    [SerializeField] PopupWindow juicyTestPopup;
    [SerializeField] GameObject[] iconParticles, glowParticles;
    [SerializeField] GameObject[] windows;
    [SerializeField] Transform[] menuButtons;
    [SerializeField] Image[] menuButtonsImage;
    [SerializeField] Sprite menuButtonSelected, menuButtonUnselected;
    [NonSerialized] public EventQueue eventQueue;
    int particleIndex;
    int curWindow = 2;

    [Space(10)]
    [SerializeField] PopupWindow crownRacePopup;
    [SerializeField] PopupWindow skyAdventurePopup;
    [SerializeField] PopupWindow carRacePopup;
    [SerializeField] PopupWindow noAdsPopup;

    public event Action OnPlayEvent;
    public static event Action OnShowEvent;

    void Awake()
    {
        eventQueue = new(() => canvasGroup.interactable = false, () => canvasGroup.interactable = true);
    }

    void OnEnable()
    {
        startGamePanel.OnPlayEvent += PlayInvoke;

        if (GameManager.GameEnabled) UpdateInfo();
        else GameManager.OnInitEvent += UpdateInfo;
    }

    void OnDisable()
    {
        startGamePanel.OnPlayEvent -= PlayInvoke;
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        OnShowEvent?.Invoke();
        OnShowEvent = null;

        if (!GameData.Saves.CrownRacePopupIsShow && CrownRace.Active) AddPopup(ShowCrownRacePopup);
        if (!GameData.Saves.SkyAdventurePopupIsShow && GameData.Saves.SkyAdventureCooldown.AddMinutes(GameData.Default.eventLoseCooldown) < DateTime.Now && GameData.Saves.CurEvent == BigEventType.SkyAdventure) AddSkyAdventurePopup();
        if (!GameData.Saves.CarRacePopupIsShow && GameData.Saves.CarRaceCooldown.AddMinutes(GameData.Default.eventLoseCooldown) < DateTime.Now && GameData.Saves.CurEvent == BigEventType.CarRace) AddCarRacePopup();
        if (!GameData.Saves.NoAds && GameData.Saves.NoAdsPopupLvl * 10 <= LevelManager.CompleteLevelCount) AddPopup(ShowNoAdsPopup);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        base.Hide();
    }

    public void AddSkyAdventurePopup() => AddPopup(ShowSkyAdventurePopup);
    public void AddCarRacePopup() => AddPopup(ShowCarRacePopup);

    void AddPopup(Action showAction)
    {
        if (eventQueue.Count > 0) eventQueue.onComplete += showAction;
        else showAction?.Invoke();
    }

    void ShowCrownRacePopup()
    {
        crownRacePopup.Show();
        eventQueue.onComplete -= ShowCrownRacePopup;
        GameData.Saves.CrownRacePopupIsShow = true;
        GameData.SaveProgress();
    }

    void ShowSkyAdventurePopup()
    {
        skyAdventurePopup.Show();
        eventQueue.onComplete -= ShowSkyAdventurePopup;
        GameData.Saves.SkyAdventurePopupIsShow = true;
        GameData.SaveProgress();
    }

    void ShowCarRacePopup()
    {
        carRacePopup.Show();
        eventQueue.onComplete -= ShowCarRacePopup;
        GameData.Saves.CarRacePopupIsShow = true;
        GameData.SaveProgress();
    }

    void ShowNoAdsPopup()
    {
        noAdsPopup.Show();
        eventQueue.onComplete -= ShowNoAdsPopup;
        GameData.Saves.NoAdsPopupLvl++;
        GameData.SaveProgress();
    }

    void UpdateInfo()
    {
        iconParticles[particleIndex].SetActive(false);
        glowParticles[particleIndex].SetActive(false);

        if (GameData.Default.juicyTestStartLvl <= LevelManager.CompleteLevelCount || GameData.Default.juicyTestStartLvl <= LevelManager.Default.CurrentLevelIndex + 1)
        {
            juicyTestSlider.Init();
            juicyTestSlider.gameObject.SetActive(true);
            juicyTestInfo.gameObject.SetActive(false);
            if (!GameData.Saves.JuicyTestPopupIsShow)
            {
                juicyTestPopup.Show();
                GameData.Saves.JuicyTestPopupIsShow = true;
                GameData.SaveProgress();
            }
            particleIndex = GameData.Saves.JuicyTestCurType + 1;
        }
        else
        {
            juicyTestInfoTxt.StringReference.Arguments = new object[] { GameData.Default.juicyTestStartLvl };
            particleIndex = 0;
        }

        iconParticles[particleIndex].SetActive(true);
        glowParticles[particleIndex].SetActive(true);
    }

    public void SetWindow(int i)
    {
        if (i == curWindow) return;

        canvasGroup.interactable = false;
        UIAnimation.SlideIn(windows[curWindow], Vector2.right * Mathf.Sign(curWindow - i));
        UIAnimation.SlideOut(windows[i], Vector2.right * Mathf.Sign(i - curWindow), 0, () => canvasGroup.interactable = true);
        menuButtons[curWindow].localScale = Vector3.one;
        menuButtonsImage[curWindow].sprite = menuButtonUnselected;
        menuButtons[i].localScale = Vector3.one * 1.3f;
        menuButtonsImage[i].sprite = menuButtonSelected;
        curWindow = i;
    }

    public void Play()
    {
        if (GameData.Default.abilitiesUnlockLvl[(int)AbilityType.Thunder] > LevelManager.CompleteLevelCount + 1 && GameData.Default.abilitiesUnlockLvl[(int)AbilityType.Thunder] > LevelManager.Default.CurrentLevelIndex + 1) PlayInvoke();
        else startGamePanel.Show();
    }

    void PlayInvoke() => OnPlayEvent?.Invoke();

    public void AddEvent(MyEvent myEvent) => eventQueue.Add(myEvent);
}