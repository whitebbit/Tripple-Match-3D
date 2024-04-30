using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using YG;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    #region Singletone
    private static LevelManager _default;
    public static LevelManager Default { get => _default; }
    public LevelManager() => _default = this;
    #endregion

    public bool editorMode = false;
    public static int CurrentLevel { get { return GameData.Saves.CurrentLevel + 1; } set { GameData.Saves.CurrentLevel = value; GameData.SaveProgress(); } }
    public static int CompleteLevelCount { get { return GameData.Saves.CompleteLevelCount; } set { GameData.Saves.CompleteLevelCount = value; GameData.SaveProgress(); } }
    public int CurrentLevelIndex;
    [SerializeField] LvlsList levels;
    public List<Level> Levels => levels.lvls;

    public event Action OnLevelStarted;


    public void Init()
    {
#if !UNITY_EDITOR
        editorMode = false;
#endif
        if (!editorMode) SelectLevel(GameData.Saves.LastLevelIndex, true);

        if (GameData.Saves.LastLevelIndex != GameData.Saves.CurrentLevel)
        {
            GameData.Saves.CurrentAttempt = 0;
            GameData.SaveProgress();
        }
    }

    private void OnDestroy()
    {
        GameData.Saves.LastLevelIndex = CurrentLevelIndex;
        GameData.SaveProgress();
    }

    private void OnApplicationQuit()
    {
        GameData.Saves.LastLevelIndex = CurrentLevelIndex;
        GameData.SaveProgress();
    }


    public void StartLevel()
    {
        OnLevelStarted?.Invoke();
    }

    public void RestartLevel()
    {
        SelectLevel(CurrentLevelIndex, false);
    }

    public void NextLevel()
    {
        if (!editorMode) CurrentLevel++;
        SelectLevel(CurrentLevelIndex + 1);
    }

    public void SelectLevel(int levelIndex, bool indexCheck = true)
    {
        if (indexCheck)
            levelIndex = GetCorrectedIndex(levelIndex);

        if (Levels[levelIndex] == null)
        {
            Debug.Log("<color=red>There is no prefab attached!</color>");
            return;
        }

        var level = Levels[levelIndex];

        if (level)
        {
            SelLevelParams(level);
            CurrentLevelIndex = levelIndex;
        }
    }

    public void PrevLevel() =>
        SelectLevel(CurrentLevelIndex - 1);

    private int GetCorrectedIndex(int levelIndex)
    {
        if (editorMode)
            return levelIndex > Levels.Count - 1 || levelIndex <= 0 ? 0 : levelIndex;
        else
        {
            int levelId = GameData.Saves.CurrentLevel;
            if (levelId > Levels.Count - 1)
            {
                if (levels.randomizedLvls)
                {
                    List<int> lvls = Enumerable.Range(0, levels.lvls.Count).ToList();
                    lvls.RemoveAt(CurrentLevelIndex);
                    return lvls[UnityEngine.Random.Range(0, lvls.Count)];
                }
                else return levelIndex >= levels.lvls.Count ? levels.loopStartLvl + (levelIndex - levels.lvls.Count) % (levels.lvls.Count - levels.loopStartLvl) : levelIndex;
            }
            return levelId;
        }
    }

    private void SelLevelParams(Level level)
    {
        if (level)
        {
            ClearChilds();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Instantiate(level, transform);
            }
            else
            {
                PrefabUtility.InstantiatePrefab(level, transform);
            }
            foreach (IEditorModeSpawn child in GetComponentsInChildren<IEditorModeSpawn>())
                child.EditorModeSpawn();
#else
            Instantiate(level, transform);
#endif
        }
    }

    private void ClearChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject destroyObject = transform.GetChild(i).gameObject;
            DestroyImmediate(destroyObject);
        }
    }
}