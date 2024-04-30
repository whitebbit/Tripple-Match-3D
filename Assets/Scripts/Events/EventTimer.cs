using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class EventTimer : MonoBehaviour
{
    LocalizeStringEvent timer;
    static Dictionary<string, string> time = new Dictionary<string, string> { { "d", "0" }, { "h", "0" }, { "m", "0" } };
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
            TimeSpan t = GameData.Saves.OldEventTime.AddMinutes(GameData.Default.eventDuration) - DateTime.Now;
            time["d"] = t.Days.ToString();
            time["h"] = t.Hours.ToString();
            time["m"] = t.Minutes.ToString();
            timer.StringReference.Arguments = new object[] { time };
            timer.RefreshString();
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
