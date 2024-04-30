using UnityEngine;
using UnityEngine.Localization.Components;

public class LocalizeStringSetLvl : MonoBehaviour
{
    LocalizeStringEvent localizeStringEvent;

    void Awake()
    {
        localizeStringEvent = GetComponent<LocalizeStringEvent>();
    }
    
    void OnEnable()
    {
        UpdateTxt();
        GameManager.OnStartMenu += UpdateTxt;
        GameManager.OnRestartEvent += UpdateTxt;
    }

    void OnDisable()
    {
        GameManager.OnStartMenu -= UpdateTxt;
        GameManager.OnRestartEvent -= UpdateTxt;
    }

    void UpdateTxt()
    {
        localizeStringEvent.StringReference.Arguments = new object[] { (LevelManager.CompleteLevelCount < LevelManager.Default.Levels.Count ? LevelManager.Default.CurrentLevelIndex : LevelManager.CompleteLevelCount) + 1 };
        localizeStringEvent.RefreshString();
    }
}
