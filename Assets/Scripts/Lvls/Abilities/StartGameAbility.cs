using DG.Tweening;
using UnityEngine;

public class StartGameAbility : Ability
{
    [SerializeField] GameObject checkMark;
    [SerializeField] GameObject infinity;
    bool select;

    protected override void Enable()
    {
        base.Enable();
        if (!IsAvailable)
        {
            checkMark.SetActive(false);
            select = false;
        }
        else infinity.SetActive(IsInfinity);
    }

    public void Click()
    {
        if (select)
        {
            checkMark.SetActive(false);
            select = false;
        }
        else
        {
            if (!IsAvailable) return;
            checkMark.SetActive(true);
            select = true;
        }
    }

    public override void TryUse()
    {
        if (select) GameManager.OnStartGame += StartGameUse;
    }

    void StartGameUse()
    {
        DOTween.Sequence().SetDelay(0.33f).OnComplete(() => base.TryUse());
        GameManager.OnStartGame -= StartGameUse;
    }

    protected override bool IsAvailable => (GameData.Saves.AbilityCount[(int)type] > 0 && available) || IsInfinity;
    protected virtual bool IsInfinity => false;
}