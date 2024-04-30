using System;
using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static BigEventType CurEvent;

    void Awake()
    {
        if (GameManager.GameEnabled) Init();
        else GameManager.OnInitEvent += Init;
    }

    void Init()
    {
        if (Available) Enable();
        else GameManager.OnRestartEvent += Check;
    }

    void Check()
    {
        if (Available)
        {
            Enable();
            GameManager.OnRestartEvent -= Check;
        }
    }

    void Enable()
    {
        if (GameData.Saves.OldEventTime.AddMinutes(GameData.Default.eventDuration) <= DateTime.Now) SwitchEvent();
        UISkyAdventureButton.Instance.gameObject.SetActive(GameData.Saves.CurEvent == BigEventType.SkyAdventure);
        UICarRaceButton.Instance.gameObject.SetActive(GameData.Saves.CurEvent == BigEventType.CarRace);

        if (GameData.Saves.SkyAdventureLvl >= 0) SkyAdventureWindow.Instance.Init();
        if (GameData.Saves.CarRaceLvl >= 0) CarRaceWindow.Instance.Init();

        StartCoroutine(EventTimer());
    }

    void SwitchEvent()
    {
        switch (GameData.Saves.CurEvent)
        {
            case BigEventType.CarRace:
                {
                    CarRaceWindow.Instance.ClearEvent();
                    GameData.Saves.CarRaceCooldown = DateTime.MinValue;
                    SetEvent(BigEventType.SkyAdventure);
                }
                break;
            case BigEventType.SkyAdventure:
                {
                    SkyAdventureWindow.Instance.EndEvent();
                    GameData.Saves.SkyAdventureCooldown = DateTime.MinValue;
                    SetEvent(BigEventType.CarRace);
                }
                break;
            default:
                {
                    SetEvent(UnityEngine.Random.Range(0, 2) == 0 ? BigEventType.SkyAdventure : BigEventType.CarRace);
                }
                break;
        }
        GameData.Saves.OldEventTime = DateTime.Now;
        GameData.SaveProgress();
    }

    IEnumerator EventTimer()
    {
        while (true)
        {
            if (GameData.Saves.OldEventTime.AddMinutes(GameData.Default.eventDuration) <= DateTime.Now)
            {
                Enable();
                yield break;
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }

    void SetEvent(BigEventType eventType)
    {
        switch (eventType)
        {
            case BigEventType.CarRace:
                {
                    GameData.Saves.CurEvent = BigEventType.CarRace;
                    UIManager.Instance.startWindow.AddCarRacePopup();
                }
                break;
            case BigEventType.SkyAdventure:
                {
                    GameData.Saves.CurEvent = BigEventType.SkyAdventure;
                    UIManager.Instance.startWindow.AddSkyAdventurePopup();
                }
                break;
        }
    }

    bool Available => GameData.Default.eventStartLvl <= LevelManager.CompleteLevelCount || GameData.Default.eventStartLvl <= LevelManager.Default.CurrentLevelIndex + 1;
}

public enum BigEventType
{
    None, SkyAdventure, CarRace
}