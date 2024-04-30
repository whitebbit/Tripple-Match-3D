using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class Ability : MonoBehaviour
{
    [SerializeField] protected AbilityType type;
    [SerializeField] TextMeshProUGUI counter;
    [SerializeField] GameObject icon, lockIcon;
    [SerializeField] LocalizeStringEvent lockLvlCounter;
    protected bool available;

    public event Action UseEvent;

    void OnEnable()
    {
        Enable();
        GameManager.PurchaseSuccessEvent += (x) => UpdateCounter();
    }
    protected virtual void Enable()
    {
        available = !(GameData.Default.abilitiesUnlockLvl[(int)type] > LevelManager.CompleteLevelCount + 1 && GameData.Default.abilitiesUnlockLvl[(int)type] > LevelManager.Default.CurrentLevelIndex + 1);
        icon.SetActive(available);
        lockIcon.SetActive(!available);
        if (available) UpdateCounter();
        else
        {
            lockLvlCounter.StringReference.Arguments = new object[] { GameData.Default.abilitiesUnlockLvl[(int)type] };
            lockLvlCounter.StringReference.RefreshString();
        }
    }

    public virtual void TryUse()
    {
        if (!IsAvailable) return;
        Use();
    }

    protected virtual void Use()
    {
        UseEvent?.Invoke();

        GameData.Saves.AbilityCount[(int)type]--;
        GameData.SaveProgress();
        UpdateCounter();
    }

    void UpdateCounter() => counter.text = GameData.Saves.AbilityCount[(int)type].ToString();

    protected virtual bool IsAvailable => GameData.Saves.AbilityCount[(int)type] > 0 && available;
}

public enum AbilityType
{
    Magnet, Return, Raise, FreezeTime, Thunder, AddTime
}