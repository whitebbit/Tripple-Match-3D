using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class UIJuicyTestSlider : MonoBehaviour
{
    #region Singletone
    private static UIJuicyTestSlider _instance;
    public static UIJuicyTestSlider Instance { get => _instance; }
    public UIJuicyTestSlider() => _instance = this;
    #endregion

    JuicyTestData data;

    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI sliderCounter;
    [SerializeField] Image itemIcon;
    [SerializeField] Transform itemIconTransform;
    [SerializeField] Transform rewardIconTransform;
    [SerializeField] LocalizeStringEvent timer;
    [SerializeField] CanvasGroup getRewardBackground;
    [SerializeField] TextMeshProUGUI getRewardTxt;
    [SerializeField] ParticleSystem showRewardParticles, getRewardParticles;
    [SerializeField] JuicyTestPopup juicyTestPopup;

    RewardIcon curRewardIcon;
    int addScore, scoreMultiplyer = 1;
    float oldValue;
    MyEvent myEvent;
    Tween sliderTween;
    JuicyTestReward curReward;

    Coroutine testTimer;
    Coroutine x2ScoreTimer;

    bool init;

    public void Init()
    {
        if (init) return;
        init = true;

        data = GameData.Default.juicyTestData;
        myEvent = new();
        if (GameData.Saves.JuicyTestCurType == -1)
        {
            GameData.Saves.JuicyTestCurType = UnityEngine.Random.Range(0, data.types.Length);
            GameData.Saves.OldJuicyTestTime = DateTime.Now;
        }
        UpdateType();

        if (GameData.Saves.X2JuicyTestScoreTime > DateTime.Now) SetX2Score();

        CheckScore();
    }

    void OnEnable()
    {
        testTimer = StartCoroutine(CheckTime());
        CheckScore();
    }

    void OnDisable()
    {
        if (testTimer != null) StopCoroutine(testTimer);
    }

    void CheckScore()
    {
        if (!data) return;
        if (addScore > 0) UIManager.Instance.startWindow.AddEvent(myEvent.OnStart(EarnScore));
        else UIManager.Instance.startWindow.AddEvent(myEvent.OnStart(UpdateSlider));
    }

    public void AddScore(int count) => addScore += count * scoreMultiplyer;

    IEnumerator CheckTime()
    {
        while (true)
        {
            if (GameData.Saves.OldJuicyTestTime.AddMinutes(GameData.Default.juicyTestDurationInMinute) <= DateTime.Now)
            {
                addScore = 0;
                GameData.Saves.OldJuicyTestTime = DateTime.Now;
                GameData.Saves.JuicyTestCurType = (GameData.Saves.JuicyTestCurType + 1) % data.types.Length;
                GameData.Saves.JuicyTestCurValue = 0;
                GameData.Saves.JuicyTestCurLvl = 0;
                GameData.Saves.JuicyTestPopupIsShow = false;
                GameData.SaveProgress();

                UpdateType();
            }

            TimeSpan time = GameData.Saves.OldJuicyTestTime.AddMinutes(GameData.Default.juicyTestDurationInMinute) - DateTime.Now;
            timer.StringReference.Arguments = new object[] { new Dictionary<string, string> { { "d", time.Days.ToString() }, { "h", time.Hours.ToString() }, { "m", time.Minutes.ToString() } } };
            timer.StringReference.RefreshString();

            yield return null;
        }
    }

    void UpdateType()
    {
        itemIcon.sprite = data.types[GameData.Saves.JuicyTestCurType].icon;

        if (GameData.Saves.JuicyTestCurLvl < 30)
        {
            int lvl = GameData.Saves.JuicyTestCurLvl;
            var reward = data.rewards[lvl];
            int value = GameData.Saves.JuicyTestCurValue;
            int targetScore = reward.targetScore;
            slider.value = (float)value / targetScore;
            sliderCounter.text = $"{value}/{targetScore}";
            oldValue = Mathf.Min(value, targetScore);

            SetReward(reward, rewardIconTransform);
        }
        else
        {
            if (curRewardIcon) Destroy(curRewardIcon.gameObject);
            slider.value = 1;
            sliderCounter.text = "";
        }

        juicyTestPopup.Init();
    }

    void SetReward(Reward reward, Transform parent)
    {
        if (curRewardIcon) Destroy(curRewardIcon.gameObject);
        curRewardIcon = reward.Instantiate(parent);
    }

    void UpdateSliderValue(float cur)
    {
        int max = data.rewards[GameData.Saves.JuicyTestCurLvl].targetScore;
        slider.value = cur / max;
        sliderCounter.text = $"{(int)cur}/{max}";
        oldValue = cur;
    }

    void EarnScore()
    {
        GameData.Saves.JuicyTestCurValue += addScore;
        GameData.SaveProgress();

        Vector3 pos = Vector2.down * 400;
        DOTween.Sequence().SetDelay(0.33f).OnComplete(() =>
        {
            Instantiate(GameData.Default.shineGlowParticles, UIManager.Instance.canvas).transform.localPosition = pos;
            UIManager.GetBonus(data.types[GameData.Saves.JuicyTestCurType].icon, pos, itemIconTransform.position, addScore, 60, () =>
            {
                UpdateSlider();
                ParticleSystem particles = Instantiate(GameData.Default.shineGlowParticles, itemIconTransform.parent);
                Transform particlesTransform = particles.transform;
                particlesTransform.localPosition = itemIconTransform.localPosition;
                particlesTransform.localScale *= 0.75f;
                particlesTransform.SetSiblingIndex(itemIconTransform.GetSiblingIndex());
                Destroy(particles.gameObject, particles.main.duration);
            });
            addScore = 0;
        });
    }

    void UpdateSlider()
    {
        if (GameData.Saves.JuicyTestCurLvl >= 30) myEvent.Complete();
        else
        {
            float value = Mathf.Min(GameData.Saves.JuicyTestCurValue, data.rewards[GameData.Saves.JuicyTestCurLvl].targetScore);
            if (oldValue == value) CheckLvl();
            else
            {
                sliderTween.Kill();
                sliderTween = DOTween.To(UpdateSliderValue, oldValue, value, Mathf.Lerp(0.33f, 1, (value - oldValue) / 5f)).OnComplete(CheckLvl);
            }
        }
    }

    void CheckLvl()
    {
        if (GameData.Saves.JuicyTestCurLvl >= 30)
        {
            myEvent.Complete();
            return;
        }

        UpdateSliderValue(Mathf.Min(GameData.Saves.JuicyTestCurValue, data.rewards[GameData.Saves.JuicyTestCurLvl].targetScore));

        var reward = data.rewards[GameData.Saves.JuicyTestCurLvl];
        if (GameData.Saves.JuicyTestCurValue >= reward.targetScore)
        {
            GetReward(reward);
            GameData.Saves.JuicyTestCurValue -= reward.targetScore;
            GameData.Saves.JuicyTestCurLvl++;
            oldValue = 0;
        }
        else myEvent.Complete();
    }

    void GetReward(JuicyTestReward reward)
    {
        UIAnimation.Appear(getRewardBackground, duration: 0.2f);
        curRewardIcon.thisTransform.SetParent(UIManager.Instance.startWindow.transform);
        curRewardIcon.thisTransform.DOScale(curRewardIcon.thisTransform.localScale * 1.5f, 0.4f);
        curRewardIcon.thisTransform.DOLocalMove(Vector3.up * 175, 0.4f).OnComplete(() => { UIAnimation.Appear(getRewardTxt); showRewardParticles.Play(); });
        curReward = reward;
    }

    public void GetReward()
    {
        if (!getRewardTxt.gameObject.activeInHierarchy) return;
        SoundHolder.Default.PlayFromSoundPack("Reward");
        getRewardParticles.Play();
        curRewardIcon.thisTransform.DOScale(curRewardIcon.thisTransform.localScale * 1.45f, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
        {
            SetReward(data.rewards[GameData.Saves.JuicyTestCurLvl], UIManager.Instance.startWindow.transform);
            curRewardIcon.thisTransform.localScale *= 1.5f;
            curRewardIcon.thisTransform.localPosition = Vector3.up * 175;
            curRewardIcon.thisTransform.DOScale(curRewardIcon.thisTransform.localScale * 1.45f, 0.4f).SetEase(Ease.OutBack).From().OnComplete(() =>
            {
                UIAnimation.Fade(getRewardBackground, 0.2f, action: () => getRewardTxt.gameObject.SetActive(false));
                curRewardIcon.thisTransform.DOScale(curRewardIcon.thisTransform.localScale / 1.5f, 0.4f).SetDelay(0.33f);
                curRewardIcon.thisTransform.DOMove(rewardIconTransform.position, 0.4f).SetDelay(0.33f).OnComplete(() => curRewardIcon.thisTransform.SetParent(rewardIconTransform));
                UpdateSlider();
            });
        });

        curReward.Get();
    }

    public void AddX2(int time)
    {
        if (GameData.Saves.X2JuicyTestScoreTime < DateTime.Now) GameData.Saves.X2JuicyTestScoreTime = DateTime.Now.AddMinutes(time);
        else GameData.Saves.X2JuicyTestScoreTime = GameData.Saves.X2JuicyTestScoreTime.AddMinutes(time);
        SetX2Score();
        GameData.SaveProgress();
    }

    void SetX2Score()
    {
        scoreMultiplyer = 2;
        if (x2ScoreTimer != null) GameManager.Instance.StopCoroutine(x2ScoreTimer);
        x2ScoreTimer = GameManager.Instance.StartTimer((int)(GameData.Saves.X2JuicyTestScoreTime - DateTime.Now).TotalSeconds, () => scoreMultiplyer = 1);
    }
}
