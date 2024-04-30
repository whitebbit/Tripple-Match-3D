using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIStarsChest : UIChest
{
    [SerializeField] Transform addStars;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI addStarsCounter;

    protected override int Score() => GameData.Saves.Stars;
    protected override int NeedScore() => GameData.Default.chestNeedStars;

    protected override void AddScore(int count)
    {
        addStars.gameObject.SetActive(true);
        addStarsCounter.text = $"+{count}";
        Vector3 startPos = addStars.localPosition;
        UIAnimation.Appear(canvasGroup, 0.2f, 0.33f, () => addStars.DOLocalMoveX(0, 0.33f).SetDelay(0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            addStars.localPosition = startPos;
            addStars.gameObject.SetActive(false);
            base.AddScore(count);
        }));
    }

    protected override void OpenChest()
    {
        GameData.Saves.Stars -= GameData.Default.chestNeedStars;
        GameData.SaveProgress();
        base.OpenChest();
    }
}