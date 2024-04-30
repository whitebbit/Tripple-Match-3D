using System;
using UnityEngine;

public class CarRaceInfoPopup : PopupWindow
{
    [SerializeField] GameObject button;
    [SerializeField] GameObject timerObj;
    [SerializeField] GameObject eventTimerObj;

    void OnEnable()
    {
        bool available = GameData.Saves.CarRaceCooldown.AddMinutes(GameData.Default.eventLoseCooldown) < DateTime.Now;
        button.SetActive(available);
        eventTimerObj.SetActive(available);
        timerObj.SetActive(!available);
    }

    void Update()
    {
        if (button.activeSelf) return;
        if (GameData.Saves.CarRaceCooldown.AddMinutes(GameData.Default.eventLoseCooldown) < DateTime.Now)
        {
            button.SetActive(true);
            eventTimerObj.SetActive(true);
            timerObj.SetActive(false);
        }
    }
}
