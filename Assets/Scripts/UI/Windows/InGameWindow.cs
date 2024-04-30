using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameWindow : Window
{
    readonly static Color FreezeTimeColor = new Color(0.6f, 0.83f, 1, 1);

    [SerializeField] Transform questList;
    public TextMeshProUGUI timer;
    public ParticleSystem addTimeParticles;
    [SerializeField] Image frostScreenEffect;
    public Ability[] abilities;
    Color startTimerColor;
    float curTime;

    public event Action PauseGameEvent;

    void Awake()
    {
        startTimerColor = timer.color;
    }

    void OnEnable()
    {
        GameManager.OnRestartEvent += Hide;
        curTime = Level.Instance.time;
    }

    void OnDisable()
    {
        GameManager.OnRestartEvent -= Hide;
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        frostScreenEffect.gameObject.SetActive(false);
        timer.color = startTimerColor;
        base.Hide();
    }

    public void PauseGame() => PauseGameEvent?.Invoke();

    public Quest AddQuest(int ID, Sprite icon, int count, int order)
    {
        Quest quest = Instantiate(GameData.Default.quest, questList);
        quest.Init(ID, icon, count, order);
        return quest;
    }

    void Update()
    {
        curTime = Mathf.Lerp(curTime, Level.Instance.time, Time.deltaTime * 6);
        int time = (int)curTime;
        timer.text = string.Format("{0:0}:{1:00}", time / 60, time % 60);
    }

    public void FreezeScreen()
    {
        frostScreenEffect.gameObject.SetActive(true);
        frostScreenEffect.material.SetFloat("_Fill", 0);
        frostScreenEffect.material.DOFloat(0.75f, "_Fill", 0.33f);
        timer.color = FreezeTimeColor;
    }

    public void UnfreezeScreen()
    {
        frostScreenEffect.material.DOFloat(0, "_Fill", 0.33f).OnComplete(() => frostScreenEffect.gameObject.SetActive(false));
        timer.color = startTimerColor;
    }
}