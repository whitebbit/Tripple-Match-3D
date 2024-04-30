using DG.Tweening;

public class UILvlChest : UIChest
{
    protected override int Score() => GameData.Saves.CompleteLevelCount - (GameData.Saves.LvlChestOpen * GameData.Default.chestNeedLvl);
    protected override int NeedScore() => GameData.Default.chestNeedLvl;

    protected override void AddScore(int count)
    {
        DOTween.Sequence().SetDelay(0.5f).OnComplete(() => base.AddScore(count));
    }

    protected override void OpenChest()
    {
        GameData.Saves.LvlChestOpen++;
        GameData.SaveProgress();
        base.OpenChest();
    }
}