using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UICarRaceButton : MonoBehaviour
{
    #region Singletone
    private static UICarRaceButton _instance;
    public static UICarRaceButton Instance { get => _instance; }
    public UICarRaceButton() => _instance = this;
    #endregion

    [SerializeField] Transform addScore;
    [SerializeField] CanvasGroup addScorecanvasGroup;
    [SerializeField] TextMeshProUGUI addScoreCounter;
    [SerializeField] PopupWindow popup;
    [SerializeField] GameObject mark;
    [SerializeField] GameObject eventTimer, cooldownTimer;
    int curScore, curLvl;

    Coroutine coroutine;

    void Awake()
    {
        curScore = GameData.Saves.CarRaceScore;
        curLvl = Mathf.Max(GameData.Saves.CarRaceLvl, 0);
    }

    void OnEnable()
    {
        coroutine = StartCoroutine(UpdateTimer());

        if (curLvl != GameData.Saves.CarRaceLvl)
        {
            curScore = 0;
            curLvl = GameData.Saves.CarRaceLvl;
        }

        if (curScore < GameData.Saves.CarRaceScore)
        {
            int diff = GameData.Saves.CarRaceScore - curScore;
            curScore = GameData.Saves.CarRaceScore;

            addScore.gameObject.SetActive(true);
            addScoreCounter.text = $"+{diff}";
            Vector3 startPos = addScore.localPosition;
            UIAnimation.Appear(addScorecanvasGroup, 0, 0.33f, () => addScore.DOLocalMoveX(0, 0.33f).SetDelay(0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                addScore.localPosition = startPos;
                addScore.gameObject.SetActive(false);
            }));
        }
    }

    void OnDisable()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            bool isCooldown = GameData.Saves.CarRaceCooldown.AddMinutes(GameData.Default.eventLoseCooldown) > DateTime.Now;
            eventTimer.SetActive(!isCooldown);
            cooldownTimer.SetActive(isCooldown);
            if (GameData.Saves.CarRaceLvl == -1 && !isCooldown) mark.SetActive(true);
            yield return new WaitForSeconds(1);
        }
    }

    public void Click()
    {
        if (!GameData.Saves.CarRaceIsStarted || GameData.Saves.CarRaceCooldown.AddMinutes(GameData.Default.eventLoseCooldown) > DateTime.Now) popup.Show();
        else CarRaceWindow.Instance.AnimationShow();
    }
}