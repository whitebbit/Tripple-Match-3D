using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using YG;

public class GameManager : MonoBehaviour
{
    #region Singletone
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }
    public GameManager() => _instance = this;
    #endregion

    [SerializeField] private DataHolder[] dataHolders;

    public static bool GameEnabled;
    public static event Action OnInitEvent;
    public static event Action OnStartMenu, OnPostStartMenu;
    public static event Action OnStartGame;
    public static event Action OnRestartEvent;
    public static event Action<string> PurchaseSuccessEvent;

    GameState state;
    public GameState State
    {
        get => state;
        set
        {
            if (state == value) return;
            switch (value)
            {
                case GameState.StartMenu:
                    {
                        OnStartMenu?.Invoke();
                        UITransition.Instantiate().Open();
                        OnPostStartMenu?.Invoke();
                    }
                    break;
                case GameState.Game:
                    {
                        OnStartGame?.Invoke();
                        UITransition.Instantiate().Open();
                    }
                    break;
            }
            //
            state = value;
        }
    }

    public static bool CanWatchAD => LevelManager.CompleteLevelCount >= 2;

    void OnEnable()
    {
        GameData.ChangeHPEvent += ChangeHP;
        YandexGame.PurchaseSuccessEvent += SuccessPurchased;
    }

    void OnDisable()
    {
        GameData.ChangeHPEvent -= ChangeHP;
        YandexGame.PurchaseSuccessEvent -= SuccessPurchased;
    }

    void Start()
    {
        Application.targetFrameRate = -1;
        for (int i = 0; i < dataHolders.Length; i++) dataHolders[i].Init();

        LevelManager.Default.Init();

        UIManager.Instance.Init();

        PlayerController.Instance.Init();
        Inventory.Instance.Init();

        CameraController.Instance.Init();

        SoundHolder.Default.PlayFromSoundPack("Background Music", null, true);

        GameEnabled = true;
        OnInitEvent?.Invoke();
        State = GameState.StartMenu;

        StartCoroutine(RefillHP());
    }

    public void StartGame() => UITransition.Instantiate().Close(() => State = GameState.Game);

    public void Fail(FailType failType) => UIManager.Instance.Fail(failType);

    public void RestartLvl()
    {
        UITransition.Instantiate().Close(() =>
        {
            KillAllTweens();
            LevelManager.Default.SelectLevel(LevelManager.Default.CurrentLevelIndex);
            OnRestartEvent?.Invoke();
            state = GameState.None;
            State = GameState.Game;
        });
    }

    public void LoadPrevLvl() => LoadLvl(Mathf.Max(LevelManager.Default.CurrentLevelIndex - 1, 0));
    public void LoadMainMenu() => LoadLvl(LevelManager.Default.CurrentLevelIndex);
    public void LoadNextLvl() => LoadLvl(LevelManager.Default.CurrentLevelIndex + 1);
    public void LoadLvl(int lvl)
    {
        LevelManager.CurrentLevel = lvl;
        UITransition.Instantiate().Close(() =>
        {
            KillAllTweens();
            LevelManager.Default.SelectLevel(lvl);
            OnRestartEvent?.Invoke();
            state = GameState.None;
            State = GameState.StartMenu;
        });
    }

    void KillAllTweens()
    {
        DOTween.KillAll(true);
        DOTween.KillAll(true);
        DOTween.KillAll(true);
    }

    float timeScaleBeforePause;
    public void PauseGame()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;
        AudioListener.pause = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = timeScaleBeforePause;
        AudioListener.pause = false;
    }

    void ChangeHP(int value)
    {
        if (value < 0 && GameData.HP == 5)
        {
            GameData.Saves.OldHPTime = DateTime.Now;
            GameData.SaveProgress();
        }
    }

    IEnumerator RefillHP()
    {
        while (true)
        {
            while (GameData.HP < 5 && GameData.Saves.OldHPTime.AddSeconds(1800) <= DateTime.Now)
            {
                GameData.Default.AddHP(1);
                GameData.Saves.OldHPTime = GameData.Saves.OldHPTime.AddSeconds(1800);
                GameData.SaveProgress();
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public Coroutine StartTimer(int seconds, Action onComplete) => StartCoroutine(Timer(seconds, onComplete));
    IEnumerator Timer(int seconds, Action onComplete)
    {
        yield return new WaitForSecondsRealtime(seconds);
        onComplete?.Invoke();
    }

    void SuccessPurchased(string id)
    {
        switch (id)
        {
            case "Set_00":
                {
                    GetRewards(GameData.Default.purchasedData[0]);
                }
                break;
            case "Set_01":
                {
                    GetRewards(GameData.Default.purchasedData[1]);
                }
                break;
            case "Set_02":
                {
                    GetRewards(GameData.Default.purchasedData[2]);
                }
                break;
            case "Set_03":
                {
                    GetRewards(GameData.Default.purchasedData[3]);
                }
                break;
            case "Set_04":
                {
                    GetRewards(GameData.Default.purchasedData[4]);
                }
                break;
            case "Set_05":
                {
                    GetRewards(GameData.Default.purchasedData[5]);
                }
                break;
            case "No_Ads_00":
                {
                    GameData.Saves.NoAds = true;
                    GameData.SaveProgress();
                }
                break;
            case "No_Ads_01":
                {
                    GameData.Saves.NoAds = true;
                    GameData.SaveProgress();
                    GetRewards(GameData.Default.purchasedData[7]);
                }
                break;
            case "Coins_00":
                {
                    GetRewards(GameData.Default.purchasedData[8]);
                }
                break;
            case "Coins_01":
                {
                    GetRewards(GameData.Default.purchasedData[9]);
                }
                break;
            case "Coins_02":
                {
                    GetRewards(GameData.Default.purchasedData[10]);
                }
                break;
            case "Coins_03":
                {
                    GetRewards(GameData.Default.purchasedData[11]);
                }
                break;
            case "Coins_04":
                {
                    GetRewards(GameData.Default.purchasedData[12]);
                }
                break;
            case "Coins_05":
                {
                    GetRewards(GameData.Default.purchasedData[13]);
                }
                break;
        }

        PurchaseSuccessEvent?.Invoke(id);
    }

    void GetRewards(Reward[] rewards)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            int n = i;
            rewards[n].Get();
            DOTween.Sequence().SetDelay(0.1f * n).OnComplete(() => UIManager.GetReward(rewards[n], Vector2.down * 400, UIManager.Instance.canvas.transform.TransformPoint(Vector2.down * 1400)));
        }
    }
}

public enum GameState
{
    None, StartMenu, Game, Finish
}