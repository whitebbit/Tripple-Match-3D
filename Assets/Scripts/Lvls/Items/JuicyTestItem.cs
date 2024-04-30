using DG.Tweening;
using UnityEngine;

public class JuicyTestItem : Item
{
    public override void Collect()
    {
        base.Collect();
        Inventory.Slot slot = Inventory.Instance.slots.Find(x => x.item == this);
        if (slot != null) slot.OnCompleteCollectItem = () => CollectJuicyTestItem(slot);
    }

    void CollectJuicyTestItem(Inventory.Slot slot)
    {
        Inventory.Instance.RemoveSlot(slot);
        JuicyTestItemCounter.Instance.CollectItem(this);
        Instantiate(GameData.Default.collectSpecialItemParticles, thisTransform.position, Quaternion.identity);
        SoundHolder.Default.PlayFromSoundPack("WeeklyItemCollect");
    }
}