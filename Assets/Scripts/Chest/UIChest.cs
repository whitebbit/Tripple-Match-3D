using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIChest : MonoBehaviour
{
    [SerializeField] Window popupWindow;
    [SerializeField] Slider slider;
    int curScore;
    MyEvent myEvent;

    void Awake()
    {
        if (GameManager.GameEnabled) Score();
        else GameManager.OnInitEvent += () => curScore = Score();
        myEvent = new MyEvent();
    }

    void OnEnable()
    {
        if (GameManager.GameEnabled) UpdateScore();
        else GameManager.OnInitEvent += UpdateScore;
    }

    void UpdateScore()
    {
        if (Score() != curScore)
        {
            int addScore = Score() - curScore;
            myEvent.OnStart(() => AddScore(addScore));
            curScore = Score();
        }
        else
        {
            slider.value = (float)curScore / NeedScore();
            myEvent.OnStart(() => CheckScore());
        }
        if (curScore >= NeedScore()) UIManager.Instance.startWindow.AddEvent(myEvent);
        else myEvent.Start();
    }

    protected abstract int Score();
    protected abstract int NeedScore();
    protected virtual void AddScore(int count) => slider.DOValue((float)curScore / NeedScore(), 0.4f).OnComplete(CheckScore);
    void CheckScore()
    {
        if (curScore >= NeedScore())
        {
            OpenChestPopup.Instance.Show(GameData.Default.chestRewardData[GameData.Saves.ChestCurReward], OpenChest, CompleteOpenChest);
            GameData.Saves.ChestCurReward = (GameData.Saves.ChestCurReward + 1) % GameData.Default.chestRewardData.rewards.Length;
            GameData.SaveProgress();
        }
        else myEvent.Complete();
    }

    public void OpenPopup() => popupWindow.Show();

    protected virtual void OpenChest() => curScore -= NeedScore();

    void CompleteOpenChest()
    {
        myEvent.Complete();
        UpdateScore();
    }
}
