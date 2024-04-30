public class AbilityFreeze : Ability
{
    protected override void Use()
    {
        if (!Level.Instance.TryFreezeTime()) return;
        base.Use();
    }
}