using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class SkyAdventureCooldownTimer : MonoBehaviour
{
    LocalizeStringEvent timer;
    static Dictionary<string, string> time = new Dictionary<string, string> { { "min", "0" }, { "sec", "0" } };
    Coroutine coroutine;

    void Awake()
    {
        timer = GetComponent<LocalizeStringEvent>();
    }

    void OnEnable()
    {
        coroutine = StartCoroutine(UpdateTimer());
    }

    void OnDisable()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            TimeSpan t = GameData.Saves.SkyAdventureCooldown.AddMinutes(GameData.Default.eventLoseCooldown) - DateTime.Now;
            time["min"] = t.Minutes.ToString();
            time["sec"] = t.Seconds.ToString();
            timer.StringReference.Arguments = new object[] { time };
            timer.RefreshString();

            yield return new WaitForSecondsRealtime(1);
        }
    }
}
