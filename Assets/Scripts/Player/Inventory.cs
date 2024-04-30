using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singletone
    private static Inventory _instance;
    public static Inventory Instance { get => _instance; }
    public Inventory() => _instance = this;
    #endregion

    [Serializable]
    public class Slot
    {
        public Item item;
        public int slot;
        public Action OnCompleteCollectItem = null;

        public bool isMerged, isActiveMerge;

        public Slot(Item item, int slot)
        {
            this.item = item;
            this.slot = slot;
        }
    }

    [SerializeField] Vector3[] points;
    Vector3 Points(int n) => thisTransform.TransformPoint(points[Mathf.Min(n, points.Length - 1)]);

    [SerializeField] AnimationCurve mergeCurve;

    Transform thisTransform;
    [NonSerialized] public List<Slot> slots;
    public int FreeSlots => points.Length - slots.Count;
    int mergeProcesses, activeMergeProcesses;
    int collectProcess;

    public void Init()
    {
        thisTransform = transform;
        slots = new();

        GameManager.OnRestartEvent += Restart;
        GameManager.OnStartGame += StartGame;
    }

    void Restart()
    {
        gameObject.SetActive(false);
        slots.Clear();
    }

    void StartGame()
    {
        gameObject.SetActive(true);
        mergeProcesses = 0;
        collectProcess = 0;
        activeMergeProcesses = 0;
    }

    public Slot AddItem(Item item)
    {
        if (slots.Count >= 7)
        {
            item.Return();
            return null;
        }

        int slot = slots.FindLastIndex(x => x.item.ID == item.ID);
        if (slot == -1) slot = slots.Count; else slot++;
        Slot newSlot = new Slot(item, slot);
        slots.Insert(slot, newSlot);

        item.thisTransform.DORotateQuaternion(GameData.Default.items[item.ID].transform.rotation, 0.33f);
        StartCoroutine(CollectItem(newSlot));
        collectProcess++;

        List<Slot> mergeItems = slots.FindAll(x => x.item.ID == item.ID && !x.isMerged);
        if (mergeItems != null && mergeItems.Count == 3)
        {
            mergeProcesses++;
            mergeItems.ForEach(x => x.isMerged = true);
            newSlot.OnCompleteCollectItem = () => StartCoroutine(MergeItems(mergeItems));
        }
        else
        {
            newSlot.OnCompleteCollectItem = () =>
            {
                item.Fly();
                if (slots.Count >= points.Length && mergeProcesses == 0) Level.Instance.GameFail(FailType.InventoryFull);
            };
        }

        return newSlot;
    }

    IEnumerator CollectItem(Slot slot)
    {
        float t = 0;
        Vector3 startPos = slot.item.thisTransform.position;
        while (t < 0.33f)
        {
            t += Time.deltaTime;
            slot.item.thisTransform.position = Vector3.Lerp(startPos, Points(slots.IndexOf(slot)), t / 0.33f);
            yield return null;
        }
        slot.OnCompleteCollectItem?.Invoke();
        slot.OnCompleteCollectItem = null;
        collectProcess--;
    }

    IEnumerator MergeItems(List<Slot> mergeItems)
    {
        yield return new WaitWhile(() => collectProcess > 0);
        activeMergeProcesses++;

        mergeItems[0].isActiveMerge = true;
        mergeItems[2].isActiveMerge = true;
        Item targetItem = mergeItems[1].item;
        float dist = Vector3.Distance(targetItem.thisTransform.position, mergeItems[0].item.thisTransform.position);
        float t = 0;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            float tCurve = dist * mergeCurve.Evaluate(t / 0.4f);
            mergeItems[0].item.thisTransform.position = targetItem.thisTransform.position - Vector3.right * tCurve;
            mergeItems[2].item.thisTransform.position = targetItem.thisTransform.position + Vector3.right * tCurve;
            yield return null;
        }

        mergeItems[0].item.Destroy();
        slots.Remove(mergeItems[0]);
        mergeItems[2].item.Destroy();
        slots.Remove(mergeItems[2]);

        SoundHolder.Default.PlayFromSoundPack("Merge");
        Instantiate(GameData.Default.mergeParticles, targetItem.thisTransform.position, Quaternion.identity);
        targetItem.thisTransform.DOScale(0, 0.33f).SetEase(Ease.InBack).OnComplete(() =>
        {
            targetItem.Destroy();
            slots.Remove(mergeItems[1]);
            mergeProcesses--;
            activeMergeProcesses--;
        });
    }

    public bool TryRemoveItem()
    {
        int index = slots.FindLastIndex(x => !x.isMerged && x.OnCompleteCollectItem == null);
        if (index == -1) return false;

        slots[index].item.Return();
        slots.RemoveAt(index);
        SoundHolder.Default.PlayFromSoundPack("Eject");
        return true;
    }

    public void RemoveSlot(Slot slot) => slots.Remove(slot);

    void Update()
    {
        // if (activeMergeProcesses > 0) return;
        for (int i = 0; i < 7; i++)
        {
            if (slots.Count > i && slots[i].OnCompleteCollectItem == null && !slots[i].isActiveMerge) slots[i].item.thisTransform.position = Vector3.Lerp(slots[i].item.thisTransform.position, Points(i), Time.deltaTime * 9);
        }
    }
}