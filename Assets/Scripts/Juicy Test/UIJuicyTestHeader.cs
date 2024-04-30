using UnityEngine;
using UnityEngine.Localization.Components;

public class UIJuicyTestHeader : MonoBehaviour
{
    LocalizeStringEvent headerTxt;

    void Awake()
    {
        headerTxt = GetComponent<LocalizeStringEvent>();
    }

    void OnEnable()
    {
        headerTxt.StringReference = GameData.Default.juicyTestData.types[Mathf.Max(GameData.Saves.JuicyTestCurType, 0)].name;
        headerTxt.RefreshString();
    }
}
