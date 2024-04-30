using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class NoLivesPopup : PopupWindow
{
    [SerializeField] LocalizeStringEvent timer;
    [SerializeField] TextMeshProUGUI price;

    void OnEnable()
    {
        UpdateTimer();
        price.text = GameData.Default.refillHpPrice.ToString();
    }

    void Update() => UpdateTimer();
    void UpdateTimer()
    {
        TimeSpan time = GameData.Saves.OldHPTime.AddSeconds(1800) - DateTime.Now;
        timer.StringReference.Arguments = new object[] { new Dictionary<string, string> { { "min", time.Minutes.ToString() }, { "sec", time.Seconds.ToString() } } };
        timer.StringReference.RefreshString();
    }

    public void Refill()
    {
        if (!GameData.Default.TryPayCoins(GameData.Default.refillHpPrice))
        {
            UIManager.Instance.ShowShopPopup();
            return;
        }
        GameData.Default.AddHP(5 - GameData.HP);
        SoundHolder.Default.PlayFromSoundPack("Buy");
        Hide();
    }
}