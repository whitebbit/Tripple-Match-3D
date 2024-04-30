using System.Collections;
using Dreamteck;
using UnityEngine;

public class CheatMenu : MonoBehaviour
{
    public void GiveAllAbilities()
    {
        for (int i = 0; i < GameData.Saves.AbilityCount.Length; i++) GameData.Saves.AbilityCount[i] += 10;
        GameData.SaveProgress();
    }

    public void CompleteLvl()
    {
        StartCoroutine(CompleteLevel());
    }

    IEnumerator CompleteLevel()
    {
        while (GameManager.Instance.State == GameState.Game)
        {
            Level.Instance.TryMagnetizeItems();
            yield return null;
        }
    }

    public void UseThunder()
    {
        Level.Instance.UseThunder();
    }

    public void SkipTest()
    {
        GameData.Saves.OldJuicyTestTime = GameData.Saves.OldJuicyTestTime.AddDays(-7);
        GameData.SaveProgress();
    }

    public void SkipEvent()
    {
        GameData.Saves.OldEventTime = GameData.Saves.OldEventTime.AddDays(-7);
        GameData.SaveProgress();
    }
}
