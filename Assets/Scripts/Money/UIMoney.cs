using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.Localization;

public class UIMoney : MonoBehaviour
{
    readonly static Color HPColor = new Color(1, 0.2f, 0.2f, 1);
    readonly static Color CoinsColor = new Color(1, 0.84f, 0.14f, 1);

    #region Singletone
    private static UIMoney _instance;
    public static UIMoney Instance { get => _instance; }
    public UIMoney() => _instance = this;
    #endregion

    [Header("HP")]
    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] TextMeshProUGUI txtTime;
    [SerializeField] LocalizedString txtTimeLocalize;
    [SerializeField] GameObject infinityIcon;
    private int oldHP;
    private Tween hpColor;
    string maxName;

    [Header("Coins")]
    [SerializeField] TextMeshProUGUI coinCountText;
    public Transform coinTransform;
    int oldCoinsCount;
    private Tween coinsColor, coinJump;
    Transform coinCounterTransform;

    void OnEnable()
    {
        txtTimeLocalize.StringChanged += (x) => { maxName = x; GameData.Default.UpdateHP(); };
    }

    public void Init()
    {
        coinCounterTransform = coinCountText.transform;
        SetHP(GameData.HP);
        SetCoins(GameData.Coins);
    }

    public void SetHP(int hp)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        bool isInfinity = GameData.InfinityHP;
        infinityIcon.SetActive(isInfinity);
        txt.gameObject.SetActive(!isInfinity);

        txt.text = hp.ToString();

        if (oldHP < hp)
        {
            hpColor.Kill(true);
            hpColor = txt.DOColor(HPColor, 0.2f).From().SetUpdate(true);
        }

        oldHP = hp;
        if (hp >= 5 || isInfinity) txtTime.text = maxName;
    }

    public void SetCoins(int coins)
    {
        coinCountText.text = FormatNumber(coins);

        if (oldCoinsCount < coins)
        {
            coinsColor.Kill(true);
            coinsColor = coinCountText.DOColor(CoinsColor, 0.2f).From().SetUpdate(true);
            coinJump.Kill(true);
            coinJump = coinCounterTransform.DOLocalMoveY(25, 0.2f).From().SetUpdate(true);
        }

        oldCoinsCount = coins;
    }

    string FormatNumber(int number)
    {
        if (number < 1000) return number.ToString();

        int k = number / 1000;
        int notK = number - k * 1000;
        int h = notK / 100;
        return k + "." + h + ((notK - h * 100) / 10) + "K";
    }

    void Update()
    {
        if (oldHP < 5 || GameData.InfinityHP)
        {
            DateTime time = GameData.InfinityHP ? GameData.Saves.InfinityHPTime : GameData.Saves.OldHPTime.AddSeconds(1800);
            TimeSpan diffTime = time - DateTime.Now;
            txtTime.text = (time.Hour > 0 ? $"{(int)diffTime.TotalHours}:" : "") + diffTime.ToString(@"mm\:ss");
        }
    }
}