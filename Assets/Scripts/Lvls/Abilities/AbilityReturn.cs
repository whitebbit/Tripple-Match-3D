public class AbilityReturn : Ability
{
    protected override void Use()
    {
        if (!Inventory.Instance.TryRemoveItem()) return;
        base.Use();
    }
}