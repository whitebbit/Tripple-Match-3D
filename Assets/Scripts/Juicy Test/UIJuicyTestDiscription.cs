using UnityEngine;
using UnityEngine.Localization.Components;

public class UIJuicyTestDiscription : MonoBehaviour
{
    LocalizeStringEvent discriptionTxt;

    void Awake()
    {
        discriptionTxt = GetComponent<LocalizeStringEvent>();
    }

    void OnEnable()
    {
        discriptionTxt.StringReference = GameData.Default.juicyTestData.types[Mathf.Max(GameData.Saves.JuicyTestCurType, 0)].discription;
        discriptionTxt.RefreshString();
    }
}
