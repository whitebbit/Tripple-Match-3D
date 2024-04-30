using UnityEngine;

public class CrownRace : MonoBehaviour
{
    public static bool Active;

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
        Active = true;
        Level.OnWinEvent += (x) => AddScore();
        Level.OnFailEvent += ClearProgress;
    }

    void AddScore()
    {
        GameData.Saves.CrownRaceScore = Mathf.Min(GameData.Saves.CrownRaceScore + 1, 3);
        GameData.SaveProgress();
    }

    void ClearProgress(FailType failType)
    {
        GameData.Saves.CrownRaceScore = 0;
        GameData.SaveProgress();
    }

    bool Available => GameData.Default.crownRaceStartLvl <= LevelManager.CompleteLevelCount || GameData.Default.crownRaceStartLvl <= LevelManager.Default.CurrentLevelIndex + 1;
}