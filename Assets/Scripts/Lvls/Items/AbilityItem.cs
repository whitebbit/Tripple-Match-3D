using UnityEngine;

public class AbilityItem : Item
{
    [SerializeField] AbilityType abilityType;

    public override void Collect()
    {
        base.Collect();
        Inventory.Slot slot = Inventory.Instance.slots.Find(x => x.item == this);
        if (slot != null) slot.OnCompleteCollectItem = () => CollectAbility(slot);
    }

    void CollectAbility(Inventory.Slot slot)
    {
        Inventory.Instance.RemoveSlot(slot);
        Instantiate(GameData.Default.collectSpecialItemParticles, thisTransform.position, Quaternion.identity);
        switch (abilityType)
        {
            case AbilityType.Thunder:
                {
                    Level.Instance.UseThunder(3);
                    Destroy();
                }
                break;
            case AbilityType.AddTime:
                {
                    Level.Instance.time += 10;
                    UIManager.Instance.inGameWindow.addTimeParticles.Play();
                    Destroy();
                }
                break;
        }
    }
}