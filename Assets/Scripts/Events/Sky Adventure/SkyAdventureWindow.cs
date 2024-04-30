using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using YG;

public class SkyAdventureWindow : Window
{
    #region Singletone
    private static SkyAdventureWindow _instance;
    public static SkyAdventureWindow Instance { get => _instance; }
    public SkyAdventureWindow() => _instance = this;
    #endregion

    readonly static Color NotCompletedColor = new Color(0.2735849f, 0.2735849f, 0.2735849f, 1);
    readonly static Color CompletedColor = new Color(0.3764706f, 0.6470588f, 1, 1);
    const string EnglishLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ", RussianLetters = "АБВГДЕКЛМНОПРСТУФЦЧШЮЯ";
    public static char RandomLetter => GameData.Language == Language.RU ? RussianLetters[UnityEngine.Random.Range(0, RussianLetters.Length)] : EnglishLetters[UnityEngine.Random.Range(0, EnglishLetters.Length)];
    public static char PlayerLetter => YandexGame.playerName[0];

    [SerializeField] PopupWindow startPopup;
    [SerializeField] SkyAdventureCheckpointPopup checkpointPopup;
    [SerializeField] PopupWindow losePopup;
    [SerializeField] SkyAdventureWinWindow winPopup;
    [SerializeField] SkyAdventureCheckpointWindow checkpointExitPopup;

    [SerializeField] List<JuicyTestReward> firstCheckpointRewards, secondCheckpointRewards, endRewards;

    [Space(10)]
    [SerializeField] Image[] progressBar;
    [SerializeField] LocalizeStringEvent levelCounter;
    [SerializeField] int firstCheckpointLvl, secondCheckpointLvl;
    [SerializeField] Transform environment;
    [SerializeField] Transform islands;
    [SerializeField] GameObject background;
    int curLvl = -1;
    bool lose;

    [Space(10)]
    [SerializeField] Transform[] points;
    [SerializeField] LocalizeStringEvent playersCounter;
    [SerializeField] SkyAdventurePlayer player;
    [SerializeField] float jumpStrength;
    [SerializeField] Transform playersParent;
    public Color[] PlayerColors;
    List<SkyAdventurePlayer> players;
    int playersCount;

    [Space(10)]
    [SerializeField] Vector3 camPos;
    [SerializeField] Vector3 camRot;
    [SerializeField] float[] camPoints;

    Sequence animationSequence;

    void Awake()
    {
        for (int i = 0; i < GameData.Saves.SkyAdventureLvl + 1; i++) progressBar[i].color = CompletedColor;

        levelCounter.StringReference.Arguments = new object[] { GameData.Saves.SkyAdventureLvl };
        levelCounter.RefreshString();
        playersCounter.StringReference.Arguments = new object[] { playersCount };
        playersCounter.RefreshString();
    }

    public void Init()
    {
        if (GameData.Saves.SkyAdventureCooldown.AddMinutes(GameData.Default.eventLoseCooldown) > DateTime.Now) return;

        if (GameData.Saves.SkyAdventureLvl >= 0)
        {
            playersCount = GameData.Saves.SkyAdventurePlayersCount;
            curLvl = GameData.Saves.SkyAdventureLvl;
        }
        else
        {
            startPopup.Show();
            curLvl = 0;
            playersCount = 100;
            GameData.Saves.SkyAdventureLvl = 0;
            GameData.Saves.SkyAdventurePlayersCount = 100;
            GameData.SaveProgress();
        }
        
        players = new();
        FillPlayers();
        players[0].Init(PlayerColors[0], PlayerLetter);
        islands.localPosition = new Vector3(islands.localPosition.x, islands.localPosition.y, camPoints[curLvl / 7]);

        EventManager.CurEvent = BigEventType.SkyAdventure;
        Level.OnWinEvent += CompleteLvl;
        Level.OnFailEvent += FailLvl;
    }

    void CompleteLvl(int score)
    {
        GameData.Saves.SkyAdventureLvl++;
        GameData.SaveProgress();
    }

    void FailLvl(FailType failType)
    {
        lose = true;
    }

    void FillPlayers()
    {
        while (players.Count < Mathf.Min(playersCount, 20))
        {
            SkyAdventurePlayer newPlayer = Instantiate(player, playersParent);
            newPlayer.Init(PlayerColors[UnityEngine.Random.Range(0, PlayerColors.Length)], RandomLetter);
            newPlayer.thisTransform.position = GetPlayerPos(players.Count);
            players.Add(newPlayer);
        }
    }

    void OnEnable()
    {
        if (lose) Lose();
        if (curLvl < GameData.Saves.SkyAdventureLvl)
        {
            FillPlayers();
            animationSequence = DOTween.Sequence().SetDelay(0.33f).OnComplete(() =>
            {
                for (int i = curLvl; i < GameData.Saves.SkyAdventureLvl + 1; i++) progressBar[i].color = CompletedColor;
                curLvl = GameData.Saves.SkyAdventureLvl;
                levelCounter.StringReference.Arguments = new object[] { GameData.Saves.SkyAdventureLvl };
                levelCounter.RefreshString();

                int subCount = UnityEngine.Random.Range(3, Mathf.Max(playersCount / Mathf.Max(22 - curLvl, 1), 4));
                if (curLvl == 21 || 8 > playersCount - subCount) subCount = Mathf.Max(playersCount - 8, 0);
                playersCount -= subCount;
                GameData.Saves.SkyAdventurePlayersCount = playersCount;
                GameData.SaveProgress();
                List<SkyAdventurePlayer> diePlayers = new();
                int count = Mathf.Min(subCount, curLvl == 21 ? 12 : 4);
                for (int i = 0; i < count; i++)
                {
                    int index = UnityEngine.Random.Range(0, players.Count);
                    diePlayers.Add(players[index]);
                    players.RemoveAt(index);
                }

                animationSequence = DOTween.Sequence().OnComplete(() =>
                {
                    islands.DOLocalMoveZ(camPoints[Mathf.Min(curLvl / 7, 2)], 0.33f);
                    if (curLvl == firstCheckpointLvl) checkpointPopup.Init(firstCheckpointRewards);
                    else if (curLvl == secondCheckpointLvl) checkpointPopup.Init(secondCheckpointRewards);
                    else if (curLvl == 21) Win();
                    playersCounter.StringReference.Arguments = new object[] { playersCount };
                    playersCounter.RefreshString();
                });
                for (int i = 0; i < players.Count; i++)
                {
                    SkyAdventurePlayer player = players[i];
                    animationSequence.Insert(0.033f * i, player.thisTransform.DOJump(GetPlayerPos(i), jumpStrength, 1, 0.5f));
                }
                Vector3 fallDir = (points[curLvl].position - points[curLvl - 1].position) * 0.66f;
                for (int i = 0; i < diePlayers.Count; i++)
                {
                    SkyAdventurePlayer player = diePlayers[i];
                    animationSequence.Insert(0.033f * (players.Count + i), player.thisTransform.DOJump(player.thisTransform.position + Quaternion.Euler(0, UnityEngine.Random.Range(-45f, 45f), 0) * fallDir + Vector3.down * 10, jumpStrength * 3, 1, 1.33f)
                    // .SetEase(Ease.InQuad)
                    .OnComplete(() => Destroy(player.gameObject)));
                }
            });
        }
    }

    readonly static Dictionary<int, int> MaxRow = new Dictionary<int, int> { { 0, 1 }, { 1, 2 }, { 3, 3 }, { 6, 4 } };
    Vector3 GetPlayerPos(int index)
    {
        int n = 10 - Mathf.Abs(10 - index);
        int sub = n < 3 ? Mathf.Min(n, 1) : (n < 6 ? 3 : 6);
        float max = MaxRow[sub] - 1;
        if (n == 10) return points[curLvl].position;
        return points[curLvl].position + new Vector3((n - sub - max / 2) * 0.33f, 0, -0.45f + Mathf.Abs((index >= 10 ? 0.9f : 0) - 0.15f * max)) + Vector3.up * 0.25f;
    }

    public void AnimationShow()
    {
        UITransition.Instantiate().Close(() =>
        {
            Show();
            UIManager.Instance.startWindow.Hide();
            UITransition.Instantiate().Open();
        });
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        environment.gameObject.SetActive(true);
        background.SetActive(false);

        CameraController.Instance.thisTransform.position = camPos;
        CameraController.Instance.thisTransform.rotation = Quaternion.Euler(camRot);
        Camera.main.orthographicSize = 8;
    }

    public override void Hide()
    {
        if (animationSequence.IsActive() && animationSequence.IsPlaying())
        {
            animationSequence.Kill(true);
            return;
        }

        UITransition.Instantiate().Close(() =>
        {
            base.Hide();
            gameObject.SetActive(false);
            background.SetActive(true);
            environment.gameObject.SetActive(false);
            CameraController.Instance.ReturnToDefault();
            Camera.main.orthographicSize = 5.36f;
            UIManager.Instance.startWindow.gameObject.SetActive(true);
            UITransition.Instantiate().Open();
        });
    }

    public void CheckpointExit()
    {
        if (curLvl == firstCheckpointLvl)
        {
            List<JuicyTestReward> rewards = firstCheckpointRewards.Random(2);
            rewards.ForEach(x => x.Get());
            checkpointExitPopup.Init(rewards);
            EndEvent();
        }
        else
        {
            List<JuicyTestReward> rewards = secondCheckpointRewards.Random(2);
            rewards.ForEach(x => x.Get());
            checkpointExitPopup.Init(rewards);
            EndEvent();
        }
    }

    void Win()
    {
        endRewards.ForEach(x => x.Get());
        winPopup.Init(endRewards, players);
        EndEvent();
    }

    public void Lose()
    {
        lose = false;
        losePopup.Show();
        EndEvent();
    }

    public void EndEvent()
    {
        for (int i = 0; i <= curLvl; i++) progressBar[i].color = NotCompletedColor;
        curLvl = -1;
        playersCount = 0;

        GameData.Saves.SkyAdventureCooldown = DateTime.Now;
        GameData.Saves.SkyAdventureLvl = -1;
        GameData.Saves.SkyAdventurePopupIsShow = false;
        GameData.SaveProgress();

        Level.OnWinEvent -= CompleteLvl;
        Level.OnFailEvent -= FailLvl;
        EventManager.CurEvent = BigEventType.None;
    }
}
