using System;
using UnityEngine;

public class SkyAdventureInfoPopup : PopupWindow
{
    [SerializeField] GameObject button;
    [SerializeField] GameObject timerObj;
    [SerializeField] GameObject eventTimerObj;

    void OnEnable()
    {
        bool available = GameData.Saves.SkyAdventureCooldown.AddMinutes(GameData.Default.eventLoseCooldown) < DateTime.Now;
        button.SetActive(available);
        eventTimerObj.SetActive(available);
        timerObj.SetActive(!available);
    }

    void Update()
    {
        if (button.activeSelf) return;
        if (GameData.Saves.SkyAdventureCooldown.AddMinutes(GameData.Default.eventLoseCooldown) < DateTime.Now)
        {
            button.SetActive(true);
            eventTimerObj.SetActive(true);
            timerObj.SetActive(false);
        }
    }
}
