using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    #region Singletone
    private static Level _instance;
    public static Level Instance { get => _instance; }
    public Level() => _instance = this;
    #endregion

    private LevelState state = LevelState.Start;
    public LevelState State
    {
        get => state;
        set
        {
            state = value;
            OnChangeState?.Invoke(value);
        }
    }

    [Serializable]
    public class Items
    {
        [ItemType] public int item;
        public int count;
        public bool quest;
    }
    public List<Items> items;
    public float time = 300;
    Transform thisTransform;
    [NonSerialized] public List<Item> curItems;
    [NonSerialized] public List<Quest> quests;
    bool freezeTime;
    float startTime;

    public LevelDifficult difficulty;

    public static event Action<LevelState> OnChangeState;
    public static event Action<int> OnWinEvent;
    public static event Action<FailType> OnFailEvent;

    public List<Reward> rewards { get; private set; }

    void OnEnable()
    {
        GameManager.OnStartMenu += Menu;
        GameManager.OnStartGame += GameStart;
    }

    void OnDisable()
    {
        GameManager.OnStartMenu -= Menu;
        GameManager.OnStartGame -= GameStart;
    }

    void Awake()
    {
        thisTransform = transform;
        curItems = new();
        quests = new();
        rewards = new();
        startTime = time;
    }

    void Menu() => State = LevelState.Start;

    public void GameStart()
    {
        curItems.Clear();
        quests.Clear();

        int juicyTestItem = -1;
        if (GameData.Saves.JuicyTestCurType >= 0)
        {
            juicyTestItem = GameData.Default.juicyTestData.types[GameData.Saves.JuicyTestCurType].item;
            items.Add(new Items() { item = juicyTestItem, count = 3 });
        }

        if (CrownRace.Active)
        {
            for (int i = 0; i < GameData.Default.crownRaceRewardItems.Length; i++)
            {
                items.Add(new Items() { item = GameData.Default.crownRaceRewardItems[i].item, count = GameData.Saves.CrownRaceScore });
            }
        }

        int itemCount = 0;
        for (int i = 0; i < items.Count; i++) itemCount += items[i].count;

        Vector2 spawnFrame = GameData.Default.itemsSpawnFrame;
        Vector2Int spawnFrameCount = GameData.Default.itemsSpawnFrameCount;
        int layersCount = itemCount / (spawnFrameCount.x * spawnFrameCount.y) + 1;
        float xStep = spawnFrame.x / (spawnFrameCount.x - 1);
        float zStep = spawnFrame.y / (spawnFrameCount.y - 1);
        List<Vector3> points = new();
        for (int layer = 0; layer < layersCount; layer++)
            for (int x = 0; x < spawnFrameCount.x; x++)
                for (int z = 0; z < spawnFrameCount.y; z++)
                    points.Add(new Vector3(-spawnFrame.x * 0.5f + xStep * x, 0.5f + layer * 0.8f, -spawnFrame.y * 0.5f + zStep * z) + UnityEngine.Random.insideUnitSphere * 0.4f);

        for (int i = 0; i < items.Count; i++)
        {
            int count = items[i].count;
            int itemID = items[i].item;
            Item item = GameData.Default.items[itemID];
            Quest quest = null;
            if (items[i].quest) quest = AddQuest(itemID, GameData.Default.itemsIcon[itemID], count, i);
            for (int j = 0; j < count; j++)
            {
                int pointIndex = UnityEngine.Random.Range(0, points.Count);
                Item curItem = Instantiate(item, points[pointIndex], UnityEngine.Random.rotation, thisTransform);
                points.RemoveAt(pointIndex);
                curItem.Init(itemID);

                if (items[i].quest)
                {
                    curItem.OnCollectEvent += (x) => quest.Reduce();
                    curItem.OnReturnEvent += (x) => quest.Return();
                    curItem.isQuest = true;
                }
                curItem.OnCollectEvent += (x) => Inventory.Instance.AddItem(x);
                curItem.OnCollectEvent += (x) => curItems.Remove(x);
                curItem.OnReturnEvent += (x) => curItems.Add(x);
                curItems.Add(curItem);
            }
        }

        State = LevelState.Process;
    }

    Quest AddQuest(int ID, Sprite icon, int count, int order)
    {
        Quest quest = UIManager.Instance.inGameWindow.AddQuest(ID, icon, count, order);
        quests.Add(quest);
        quest.OnCompleteEvent += CompleteQuest;
        return quest;
    }

    void CompleteQuest(Quest quest)
    {
        quests.Remove(quest);
        if (quests.Count <= 0) GameWin();
    }

    public bool TryMagnetizeItems()
    {
        if (quests.Count == 0 || Inventory.Instance.FreeSlots < 3) return false;
        List<Item> mergeItems = curItems.FindAll(x => x.ID == quests[0].ID);
        int count = quests[0].count % 3;
        if (count == 0) count = 3;
        for (int i = 0; i < count; i++) mergeItems[i].Collect();
        return true;
    }

    public void RaiseItems()
    {
        Instantiate(GameData.Default.windParticles, Vector3.zero, Quaternion.Euler(-90, 0, 0));
        SoundHolder.Default.PlayFromSoundPack("Shuffle");
        DOTween.Sequence().SetDelay(0.1f).OnComplete(() =>
        {
            for (int i = 0; i < curItems.Count; i++)
            {
                float f = UnityEngine.Random.Range(0, Mathf.PI * 2);
                curItems[i].thisRigidbody.velocity += new Vector3(Mathf.Cos(f), UnityEngine.Random.Range(0.33f, 1.0f), Mathf.Sin(f)) * 9;
            }
        });
    }

    public bool TryFreezeTime()
    {
        if (freezeTime) return false;
        freezeTime = true;
        UIManager.Instance.inGameWindow.FreezeScreen();
        DOTween.Sequence().SetDelay(GameData.Default.freezeTimeAbilityDuration).OnComplete(() =>
        {
            freezeTime = false;
            UIManager.Instance.inGameWindow.UnfreezeScreen();
        });
        SoundHolder.Default.PlayFromSoundPack("Freeze");
        return true;
    }

    public void UseThunder(int count = 9)
    {
        List<Item> destroyItems = new();
        for (int i = 0; i < curItems.Count && destroyItems.Count < count; i++) if (!curItems[i].isQuest && !(curItems[i] as AbilityItem) && !(curItems[i] as JuicyTestItem)) destroyItems.Add(curItems[i]);

        int destroyCount = destroyItems.Count / 3 * 3;
        for (int i = 0; i < destroyCount; i++)
        {
            Item item = destroyItems[i];
            item.gameObject.layer = 0;
            DOTween.Sequence().SetDelay(i * 0.2f).OnComplete(() =>
            {
                Instantiate(GameData.Default.lightningStrikeParticles, item.thisTransform.position, Quaternion.identity);
                item.Destroy();
                SoundHolder.Default.PlayFromSoundPack("Lighting Bolt");
            });
            curItems.Remove(item);
        }
    }

    public void AddTime(int seconds)
    {
        RewardIcon rewardIcon = UIManager.GetReward(new Reward { reward = RewardType.AddTime, count = seconds }, Vector2.zero, UIManager.Instance.inGameWindow.timer.transform.position, 0.33f, 0.5f, 1.5f, () =>
        {
            time += seconds;
            UIManager.Instance.inGameWindow.addTimeParticles.Play();
        });
        rewardIcon.counter.StringReference = GameData.Default.secondsLocalizedString;
        rewardIcon.Init(seconds);
    }

    public void AddReward(Reward reward) => rewards.Add(reward);

    public void GameWin()
    {
        if (state != LevelState.Process) return;
        State = LevelState.Win;
        LevelManager.CompleteLevelCount++;
        int stars = Mathf.Min((int)time / (int)(startTime * 0.2f), 3);
        GameData.Saves.Stars += stars;
        GameData.SaveProgress();

        OnWinEvent?.Invoke(stars);
    }

    FailType failType;
    public void GameFail(FailType failType)
    {
        if (state != LevelState.Process) return;
        State = LevelState.Fail;
        UIManager.Instance.Fail(failType);
        this.failType = failType;
    }
    public void InvokeGameFail() => OnFailEvent?.Invoke(failType);

    public void GameContinue() => State = LevelState.Process;

    void Update()
    {
        if (state != LevelState.Process || freezeTime) return;
        time -= Time.deltaTime;
        if (time <= 0) GameFail(FailType.TimeUp);
    }
}

public enum LevelState
{
    Start,
    Process,
    Win,
    Fail,
}

public enum LevelType
{
    Default, Tutorial
}

public enum LevelDifficult
{
    None, Hard, ExtraHard, VeryHard
}

public enum FailType
{
    Exit, TimeUp, InventoryFull
}