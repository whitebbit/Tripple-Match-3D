using UnityEngine;
using DG.Tweening;
using System;

public class UIManager : MonoBehaviour
{
    #region Singletone
    private static UIManager _instance;
    public static UIManager Instance { get => _instance; }
    public UIManager() => _instance = this;
    #endregion

    [Header("Main")]
    [NonSerialized] public Transform canvas;
    [SerializeField] NoLivesPopup noLivesPopup;
    [SerializeField] PopupWindow shopPopup;

    [Header("Start Window")]
    public StartWindow startWindow;

    [Header("In Game")]
    public InGameWindow inGameWindow;
    [SerializeField] PausePopup pauseWindow;

    [Header("End Windows")]
    [SerializeField] SecondAttemptPopup inventoryFullPopup;
    [SerializeField] SecondAttemptPopup timeUpPopup;
    [SerializeField] WinPopup winWindow;
    [SerializeField] LosePopup loseWindow;

    void OnEnable()
    {
        GameManager.OnStartMenu += ShowStartWindow;
        GameManager.OnStartGame += StartGame;
        Level.OnWinEvent += Win;
        Level.OnFailEvent += ShowLoseWindow;
        inGameWindow.PauseGameEvent += PauseGame;
        pauseWindow.ResumeGameEvent += ResumeGame;
        EndPopup.ReturnToMenuEvent += ReturnToMenu;
        EndPopup.RestartLvlEvent += RestartLvl;
        WinPopup.NextLvlEvent += LoadNextLvl;
        startWindow.OnPlayEvent += Play;
    }

    void OnDisable()
    {
        GameManager.OnStartMenu -= ShowStartWindow;
        GameManager.OnStartGame -= StartGame;
        Level.OnWinEvent -= Win;
        Level.OnFailEvent -= ShowLoseWindow;
        inGameWindow.PauseGameEvent -= PauseGame;
        pauseWindow.ResumeGameEvent -= ResumeGame;
        EndPopup.ReturnToMenuEvent -= ReturnToMenu;
        EndPopup.RestartLvlEvent -= RestartLvl;
        WinPopup.NextLvlEvent -= LoadNextLvl;
        startWindow.OnPlayEvent -= Play;
    }

    public void Init()
    {
        canvas = transform;
        UIMoney.Instance.Init();
    }

    void ShowStartWindow()
    {
        if (EventManager.CurEvent == BigEventType.SkyAdventure && !startWindow.gameObject.activeSelf)
        {
            SkyAdventureWindow.Instance.Show();
            return;
        }
        else if (EventManager.CurEvent == BigEventType.CarRace && !startWindow.gameObject.activeSelf)
        {
            CarRaceWindow.Instance.Show();
            return;
        }

        startWindow.Show();
    }

    public void Play()
    {
        if (GameData.HP == 0 && !GameData.InfinityHP)
        {
            noLivesPopup.Show();
            return;
        }
        GameManager.Instance.StartGame();
    }

    void StartGame()
    {
        startWindow.Hide();
        inGameWindow.Show();
    }

    void Win(int stars)
    {
        DOTween.Sequence().SetDelay(1).OnComplete(() => winWindow.Show(stars));
    }

    public void Fail(FailType failType)
    {
        switch (failType)
        {
            case FailType.InventoryFull:
                {
                    inventoryFullPopup.Show();
                }
                break;
            case FailType.TimeUp:
                {
                    timeUpPopup.Show();
                }
                break;
            case FailType.Exit:
                {
                    Level.Instance.InvokeGameFail();
                }
                break;
        }
    }

    public void ShowLoseWindow(FailType failType) => loseWindow.Show();

    void ReturnToMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }

    void RestartLvl()
    {
        if (GameData.HP == 0 && !GameData.InfinityHP)
        {
            noLivesPopup.Show();
            return;
        }
        GameManager.Instance.RestartLvl();
    }

    void LoadNextLvl()
    {
        GameManager.Instance.LoadNextLvl();
    }

    public void PauseGame()
    {
        GameManager.Instance.PauseGame();
        pauseWindow.Show();
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        pauseWindow.Hide();
    }

    public void ShowShopPopup()
    {
        shopPopup.Show();
    }

    public static void GetReward(Vector2 pos, int reward)
    {
        if (!Instance.startWindow.gameObject.activeSelf || Instance.startWindow.eventQueue.Count > 0)
        {
            Action action = null;
            action += () => GetReward(pos, reward);
            action += () => Instance.startWindow.eventQueue.onComplete -= action;
            Instance.startWindow.eventQueue.onComplete += action;
            return;
        }

        int coinCount = Mathf.Min(reward / 2, 10);
        int rewardMoney = reward;
        for (int i = 0; i < coinCount; i++)
        {
            int n = i;
            int coins = i == (coinCount - 1) ? rewardMoney : Mathf.RoundToInt(reward / (float)coinCount);
            rewardMoney -= coins;
            DOTween.Sequence().SetDelay(n * 0.05f).OnComplete(() =>
            {
                GameObject coin = Instantiate(GameData.Default.coinUI, Instance.canvas);
                Transform coinTransform = coin.transform;
                coinTransform.DOScale(0, 0.33f).From();
                coinTransform.localPosition = pos;
                coinTransform.DOLocalMove(pos + UnityEngine.Random.insideUnitCircle * 100, 0.33f).OnComplete(() =>
                coinTransform.DOMove(UIMoney.Instance.coinTransform.position, 0.25f).SetDelay(0.5f).OnComplete(() =>
                {
                    Destroy(coin);
                    GameData.Default.AddCoins(coins);
                    SoundHolder.Default.PlayFromSoundPack("Coin");
                })
                );
            });
        }

        SoundHolder.Default.PlayFromSoundPack("Reward");
    }

    public static void GetBonus(Sprite icon, Vector2 spawnPoint, Vector3 targetPoint, int count = 1, float radius = 200, Action completeStep = null)
    {
        for (int i = 0; i < count; i++)
        {
            int n = i;
            DOTween.Sequence().SetDelay(n * 0.05f).OnComplete(() =>
            {
                UIIconCounter item = Instantiate(GameData.Default.iconCounterUI, Instance.canvas);
                item.Init(icon);
                Transform itemTransform = item.transform;
                itemTransform.DOScale(0, 0.33f).From();
                itemTransform.localPosition = spawnPoint;
                itemTransform.DOLocalMove(spawnPoint + UnityEngine.Random.insideUnitCircle * radius, 0.33f).OnComplete(() =>
                {
                    itemTransform.DOMove(targetPoint, 0.4f).SetDelay(0.5f).OnComplete(() =>
                    {
                        Destroy(item.gameObject);
                        completeStep?.Invoke();
                    });
                });
            });
        }
    }

    public static RewardIcon GetReward(Reward reward, Vector2 spawnPoint, Vector3 targetPoint, float duration = 0.5f, float delay = 1, float scale = 1, Action onComplete = null)
    {
        RewardIcon rewardIcon = reward.Instantiate(Instance.canvas);
        Transform itemTransform = rewardIcon.transform;
        itemTransform.localScale *= scale;
        itemTransform.DOScale(0, duration).From();
        itemTransform.localPosition = spawnPoint;
        itemTransform.DOLocalMove(spawnPoint + UnityEngine.Random.insideUnitCircle * 200, duration).OnComplete(() =>
        {
            itemTransform.DOScale(itemTransform.localScale.x * 0.5f, duration).SetDelay(delay);
            itemTransform.DOMove(targetPoint, duration).SetDelay(delay).OnComplete(() =>
            {
                Destroy(rewardIcon.gameObject);
                onComplete?.Invoke();
            });
        });
        return rewardIcon;
    }
}