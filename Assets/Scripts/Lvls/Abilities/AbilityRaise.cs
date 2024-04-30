public class AbilityRaise : Ability
{
    protected override void Use()
    {
        Level.Instance.RaiseItems();
        base.Use();
    }
}