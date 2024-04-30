public class AbilityMagnet : Ability
{
    protected override void Use()
    {
        if (!Level.Instance.TryMagnetizeItems()) return;
        base.Use();
    }
}