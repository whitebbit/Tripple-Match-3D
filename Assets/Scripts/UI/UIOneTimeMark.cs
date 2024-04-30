using UnityEngine;

public class UIOneTimeMark : MonoBehaviour
{
    public enum OneTimeMarkType
    {
        NoAdsSale
    }

    [SerializeField] OneTimeMarkType type;

    void Awake()
    {
        if (GameData.Saves.OneTimeMarks[(int)type]) gameObject.SetActive(false);
    }

    public void Check()
    {
        GameData.Saves.OneTimeMarks[(int)type] = true;
        GameData.SaveProgress();

        gameObject.SetActive(false);
    }
}