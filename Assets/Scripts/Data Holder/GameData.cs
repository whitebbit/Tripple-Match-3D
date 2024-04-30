using System;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using YG;

[CreateAssetMenu(menuName = "Data/GameData")]
public class GameData : DataHolder
{
    #region Singleton
    private static GameData _default;
    public static GameData Default => _default;
    #endregion

    public static bool LanguageEnable;
    public static event Action OnLanguageUpdate;
    public readonly static Dictionary<string, Language> LanguageType = new Dictionary<string, Language> { { "ru", Language.RU }, { "en", Language.EN }, { "tr", Language.TR } };
    public static Language Language = Language.EN;

    public static SavesYG Saves => YandexGame.savesData;
    public static void SaveProgress() => YandexGame.SaveProgress();

    public override void Init()
    {
        _default = this;
        curUIHP = HP;

        if (YandexGame.SDKEnabled) YGInit();
        else YandexGame.GetDataEvent += YGInit;

        if (Saves.InfinityHPTime > DateTime.Now) SetInfinityHP();
    }

    void YGInit()
    {
        // #if UNITY_EDITOR
        //         Language = EditorLanguage;
        // #else
        Language = LanguageType[YandexGame.EnvironmentData.language];
        var locale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == YandexGame.EnvironmentData.language);
        if (locale) LocalizationSettings.SelectedLocale = locale;
        // #endif
        OnLanguageUpdate?.Invoke();
        LanguageEnable = true;
    }

    [Header("Editor")]
    public Language EditorLanguage;
#if UNITY_EDITOR
    public string[] itemTagsSub1;
    public string[] itemTagsSub2;
#endif

    [Space(15), Header("Settings")]
    public Vector2 itemsSpawnFrame;
    public Vector2Int itemsSpawnFrameCount;
    public LayerMask itemsLayerMask;

    [Space(8)]
    public int[] abilitiesUnlockLvl;
    public float freezeTimeAbilityDuration;
    public int addTimeAbilityValue;

    [Space(8)]
    public int juicyTestStartLvl;
    public int juicyTestDurationInMinute;

    [Serializable]
    public class CrownRaceRewardItem
    {
        [ItemType] public int item;
    }

    [Space(8)]
    public int crownRaceStartLvl;
    public CrownRaceRewardItem[] crownRaceRewardItems;

    [Serializable]
    public class CarRaceLvl
    {
        public int targetScore;
        public Reward[] rewards;
    }

    [Space(8)]
    public int eventStartLvl;
    public int eventDuration;
    public int eventLoseCooldown;
    public CarRaceLvl[] carRaceLvls;

    [Space(8)]
    public int chestNeedStars;
    public int chestNeedLvl;

    [Space(8)]
    public int refillHpPrice;

    [Space(15), Header("Visual")]
    public UITransition transition;
    public GameObject coinUI;
    public GameObject hpUI;
    public GameObject upgradeArrowUI;
    public UIIconCounter iconCounterUI;
    public Sprite[] abilitiesIcons;
    public Sprite coinsIcon;
    public Sprite[] crownRaceIcons;
    public Quest quest;
    public ParticleSystem mergeParticles;
    public ParticleSystem lightningStrikeParticles;
    public ParticleSystem windParticles;
    public ParticleSystem shineGlowParticles;
    public ParticleSystem collectSpecialItemParticles;
    public LocalizedString secondsLocalizedString;

    [Space(15), Header("Refs")]
    public List<Item> items;
    public Sprite[] itemsIcon;
    public JuicyTestData juicyTestData;
    public HighlightProfile highlightEffectProfile;
    public RewardIcon[] rewardIcons;
    public ChestRewardData chestRewardData;
    public InGamePurchasedData purchasedData;


    private int curUIHP;
    public static event Action UpdateHPEvent;
    public static event Action<int> ChangeHPEvent;

    public void UpdateHP()
    {
        UIMoney.Instance.SetHP(curUIHP);
        UpdateHPEvent?.Invoke();
    }

    public static int HP { get { return Saves.HP; } set { ChangeHPEvent?.Invoke(value - Saves.HP); Saves.HP = value; SaveProgress(); } }
    public static bool InfinityHP => Saves.InfinityHPTime > DateTime.Now;

    public void AddHP(int count)
    {
        HP += count;
        curUIHP += count;
        UpdateHP();
    }

    public void AddUIHP(int count)
    {
        curUIHP += count;
        UpdateHP();
    }

    public void UpdateUIHP()
    {
        curUIHP = HP;
        UpdateHP();
    }

    public bool PayHP(int cost)
    {
        if (!TryPayUIHP(cost)) return false;
        PayHP();
        UpdateHP();

        return true;
    }

    public void PayHP() => HP = curUIHP;

    public bool TryPayUIHP(int cost)
    {
        if (cost > curUIHP || InfinityHP) return false;

        curUIHP -= cost;
        return true;
    }

    public void AddInfinityHP(int time)
    {
        if (Saves.InfinityHPTime < DateTime.Now) Saves.InfinityHPTime = DateTime.Now.AddMinutes(time);
        else Saves.InfinityHPTime = Saves.InfinityHPTime.AddMinutes(time);
        SetInfinityHP();
        SaveProgress();
    }

    Coroutine infinityHPTimer;
    void SetInfinityHP()
    {
        Default.UpdateHP();
        if (infinityHPTimer != null) GameManager.Instance.StopCoroutine(infinityHPTimer);
        infinityHPTimer = GameManager.Instance.StartTimer((int)(Saves.InfinityHPTime - DateTime.Now).TotalSeconds, Default.UpdateHP);
    }


    public static event Action UpdateCoinsEvent;

    public void UpdateCoins()
    {
        UIMoney.Instance.SetCoins(Coins);
        UpdateCoinsEvent?.Invoke();
    }

    public static int Coins { get { return Saves.Coins; } set { Saves.Coins = value; SaveProgress(); } }

    public void AddCoins(int count)
    {
        Coins += count;
        UpdateCoins();
    }

    public bool TryPayCoins(int cost)
    {
        if (cost > Coins) return false;

        AddCoins(-cost);
        return true;
    }
}

public enum Language
{
    RU, EN, TR
}