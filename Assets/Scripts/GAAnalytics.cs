using GameAnalyticsSDK;
using UnityEngine;
using YG;

public class GAAnalytics : MonoBehaviour
{
    void Awake()
    {
        GameAnalytics.Initialize();
        GameManager.OnStartGame += StartGame;
        Level.OnWinEvent += (x) => CompleteGame();
        Level.OnFailEvent += (x) => FailGame();
        GameManager.PurchaseSuccessEvent += Purchase;
    }

    void StartGame() => GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, (LevelManager.CompleteLevelCount + 1).ToString());
    void CompleteGame() => GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, LevelManager.CompleteLevelCount.ToString());
    void FailGame() => GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, (LevelManager.CompleteLevelCount + 1).ToString());

    void Purchase(string id) => GameAnalytics.NewBusinessEvent("YAN", int.Parse(YandexGame.PurchaseByID(id).priceValue), "Shop item", id, "Shop_01");
}