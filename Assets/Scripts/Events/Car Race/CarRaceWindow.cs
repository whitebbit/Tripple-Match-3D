using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class CarRaceWindow : Window
{
    #region Singletone
    private static CarRaceWindow _instance;
    public static CarRaceWindow Instance { get => _instance; }
    public CarRaceWindow() => _instance = this;
    #endregion

    [SerializeField] PopupWindow losePopup;
    [SerializeField] GameObject getRewardButton;
    [SerializeField] LocalizeStringEvent lvlCounter;
    [SerializeField] LocalizeStringEvent discription;

    [Space(10)]
    [SerializeField] CarRacePlayer myCar;
    [SerializeField] Transform[] playersPoints;
    [SerializeField] Transform playersProfileParent;
    [SerializeField] CarRacePlayer[] cars;
    [SerializeField] CarRacePlayerProfile profile;
    [SerializeField] Sprite[] profileFrames, profileIcons;
    [SerializeField] LocalizedString[] profileNames;
    [SerializeField] CarRacePlayerLvlCounter playerLvlCounter;
    List<CarRacePlayer> players;
    List<GameObject> playersProfile;
    List<CarRacePlayerLvlCounter> playersLvlCounters;
    CarRacePlayerLvlCounter myLvlCounter;
    int curLvl = -1, curScore;
    int[] playersScore;
    bool win;

    [Space(10)]
    [SerializeField] float roadLength;

    Sequence animationSequence;

    void Awake()
    {
        if (players == null) players = new();
        if (playersProfile == null) playersProfile = new();
        if (playersLvlCounters == null) playersLvlCounters = new();
        InitPlayers();
        UpdateLvl();
    }

    public void Init()
    {
        if (GameData.Saves.CarRaceCooldown.AddMinutes(GameData.Default.eventLoseCooldown) > DateTime.Now) return;

        GameData.Saves.CarRaceIsStarted = true;

        myCar.Init();
        myLvlCounter = Instantiate(playerLvlCounter, myCar.thisTransform);
        myLvlCounter.transform.localPosition += Vector3.forward * 6;
        myLvlCounter.SetLvl(GameData.Saves.CarRaceScore);

        playersScore = new int[4];
        if (GameData.Saves.CarRaceLvl >= 0)
        {
            curLvl = GameData.Saves.CarRaceLvl;
            curScore = Mathf.Min(GameData.Saves.CarRaceScore, GameData.Default.carRaceLvls[curLvl].targetScore - 1);
            for (int i = 0; i < playersScore.Length; i++) playersScore[i] = GameData.Saves.CarRacePlayersScore[i];
        }
        else
        {
            curLvl = 0;
            GameData.Saves.CarRaceLvl = 0;
        }

        EventManager.CurEvent = BigEventType.CarRace;
        Level.OnWinEvent += CompleteLvl;
        Level.OnFailEvent += FailLvl;

        if (players == null) players = new();
        if (playersProfile == null) playersProfile = new();
        if (playersLvlCounters == null) playersLvlCounters = new();
        InitPlayers();
        UpdateLvl();

        GameData.SaveProgress();
    }

    void InitPlayers()
    {
        players.ForEach(x => Destroy(x.gameObject));
        players.Clear();
        playersProfile.ForEach(x => Destroy(x));
        playersProfile.Clear();
        playersLvlCounters.Clear();

        List<Sprite> icons = new(profileIcons);
        List<LocalizedString> names = new(profileNames);
        List<CarRacePlayer> cars = new(this.cars);
        for (int i = 0; i < 4; i++)
        {
            CarRacePlayerProfile profile = Instantiate(this.profile, playersProfileParent);
            int curFlag = UnityEngine.Random.Range(0, icons.Count);
            int curName = UnityEngine.Random.Range(0, names.Count);
            profile.Init(profileFrames[UnityEngine.Random.Range(0, profileFrames.Length)], icons[curFlag], names[curName]);
            icons.RemoveAt(curFlag);
            names.RemoveAt(curName);
            playersProfile.Add(profile.gameObject);

            int curCar = UnityEngine.Random.Range(0, cars.Count);
            CarRacePlayer car = Instantiate(cars[curCar], playersPoints[i]);
            car.Init();
            car.thisTransform.localPosition += Vector3.forward * PlayerCurrentZ(i);
            players.Add(car);
            cars.RemoveAt(curCar);

            CarRacePlayerLvlCounter lvlCounter = Instantiate(playerLvlCounter, car.thisTransform);
            lvlCounter.transform.localPosition += Vector3.forward * 6;
            lvlCounter.SetLvl(GameData.Saves.CarRacePlayersScore[i]);
            playersLvlCounters.Add(lvlCounter);
        }

        myCar.thisTransform.localPosition = Vector3.forward * CurrentZ;
    }

    void UpdateLvl()
    {
        lvlCounter.StringReference.Arguments = new object[] { curLvl + 1 };
        lvlCounter.RefreshString();
        discription.StringReference.Arguments = new object[] { GameData.Default.carRaceLvls[curLvl].targetScore };
        discription.RefreshString();
    }

    float CurrentZ => curScore >= GameData.Default.carRaceLvls[curLvl].targetScore ? roadLength * 1.5f : (float)curScore / GameData.Default.carRaceLvls[curLvl].targetScore * roadLength;
    float PlayerCurrentZ(int i) => playersScore[i] >= GameData.Default.carRaceLvls[curLvl].targetScore ? roadLength * 1.5f : (float)playersScore[i] / GameData.Default.carRaceLvls[curLvl].targetScore * roadLength;

    void CompleteLvl(int score)
    {
        if (GameData.Saves.CarRaceScore > 0) for (int i = 0; i < GameData.Saves.CarRacePlayersScore.Length; i++) if (UnityEngine.Random.Range(0, 2) == 0) GameData.Saves.CarRacePlayersScore[i]++;
        GameData.Saves.CarRaceScore = Mathf.Min(GameData.Saves.CarRaceScore + 1, GameData.Default.carRaceLvls[curLvl].targetScore);
        GameData.SaveProgress();
    }

    void FailLvl(FailType failType)
    {
        curScore = 0;
        for (int i = 0; i < GameData.Saves.CarRacePlayersScore.Length; i++) if (UnityEngine.Random.Range(0, 2) == 0) GameData.Saves.CarRacePlayersScore[i]++;
        GameData.Saves.CarRaceScore = 0;
        GameData.SaveProgress();
        myCar.thisTransform.localPosition = Vector3.zero;
    }

    void OnEnable()
    {
        bool otherPlayersMove = false;
        for (int i = 0; i < GameData.Saves.CarRacePlayersScore.Length; i++)
            if (GameData.Saves.CarRacePlayersScore[i] > playersScore[i])
            {
                otherPlayersMove = true;
                break;
            }

        if (GameData.Saves.CarRaceScore > curScore || otherPlayersMove)
        {
            animationSequence = DOTween.Sequence().SetDelay(0.2f).OnComplete(() =>
            {
                curScore = GameData.Saves.CarRaceScore;
                for (int i = 0; i < playersScore.Length; i++) playersScore[i] = GameData.Saves.CarRacePlayersScore[i];
                animationSequence = DOTween.Sequence().OnComplete(() =>
                {
                    if (GameData.Saves.CarRaceScore == GameData.Default.carRaceLvls[curLvl].targetScore) getRewardButton.SetActive(true);
                });
                animationSequence.Append(myCar.thisTransform.DOLocalMoveZ(CurrentZ, 0.5f).OnComplete(() => myLvlCounter.SetLvl(curScore)));
                for (int i = 0; i < 4; i++)
                {
                    int index = i;
                    animationSequence.Join(players[index].thisTransform.DOLocalMoveZ(PlayerCurrentZ(index), 0.5f).OnComplete(() =>
                    {
                        playersLvlCounters[index].SetLvl(playersScore[index]);
                        if (playersScore[index] >= GameData.Default.carRaceLvls[curLvl].targetScore) Lose();
                    }));
                }
            });
        }
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
        DirectionLight.Instance.SetRotation(Quaternion.Euler(45, 180, 0));
        gameObject.SetActive(true);
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
            DirectionLight.Instance.SetRotation(Quaternion.Euler(85, 180, 0));
            UIManager.Instance.startWindow.gameObject.SetActive(true);
            UITransition.Instantiate().Open();
        });
    }

    public void Win()
    {
        win = true;
        curScore = 0;
        GameData.Saves.CarRaceScore = 0;
        myLvlCounter.SetLvl(curScore);
        for (int i = 0; i < GameData.Saves.CarRacePlayersScore.Length; i++)
        {
            playersScore[i] = 0;
            GameData.Saves.CarRacePlayersScore[i] = 0;
        }
        curLvl++;
        GameData.Saves.CarRaceLvl++;
        GameData.SaveProgress();

        OpenChestPopup.Instance.Show(GameData.Default.carRaceLvls[Mathf.Min(curLvl - 1, GameData.Default.carRaceLvls.Length - 1)].rewards, null, GameData.Saves.CarRaceLvl == GameData.Default.carRaceLvls.Length ? Hide : () => UITransition.Instantiate().Close(() =>
        {
            win = false;
            getRewardButton.SetActive(false);
            InitPlayers();
            UpdateLvl();
            UITransition.Instantiate().Open();
        }));

        if (GameData.Saves.CarRaceLvl == GameData.Default.carRaceLvls.Length)
        {
            ClearEvent();
        }
    }

    void Lose()
    {
        if (curScore >= GameData.Default.carRaceLvls[curLvl].targetScore || win) return;
        losePopup.Show();
        GameData.Saves.CarRaceIsStarted = false;
        EndEvent();
    }

    void EndEvent()
    {
        getRewardButton.SetActive(false);

        win = false;
        curScore = 0;
        if (playersScore != null) for (int i = 0; i < GameData.Saves.CarRacePlayersScore.Length; i++) playersScore[i] = 0;
        if (myLvlCounter) myLvlCounter.SetLvl(curScore);

        GameData.Saves.CarRaceCooldown = DateTime.Now;
        GameData.Saves.CarRaceScore = 0;
        for (int i = 0; i < GameData.Saves.CarRacePlayersScore.Length; i++) GameData.Saves.CarRacePlayersScore[i] = 0;
        GameData.Saves.CarRacePopupIsShow = false;
        GameData.SaveProgress();

        Level.OnWinEvent -= CompleteLvl;
        Level.OnFailEvent -= FailLvl;
        EventManager.CurEvent = BigEventType.None;
    }

    public void ClearEvent()
    {
        curLvl = -1;
        GameData.Saves.CarRaceLvl = -1;
        Level.OnWinEvent -= CompleteLvl;
        Level.OnFailEvent -= FailLvl;
        GameData.Saves.CarRaceIsStarted = false;
        EndEvent();
    }
}