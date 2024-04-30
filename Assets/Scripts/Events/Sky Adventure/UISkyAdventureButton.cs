using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkyAdventureButton : MonoBehaviour
{
    #region Singletone
    private static UISkyAdventureButton _instance;
    public static UISkyAdventureButton Instance { get => _instance; }
    public UISkyAdventureButton() => _instance = this;
    #endregion

    [SerializeField] Slider slider;
    [SerializeField] Transform addScore;
    [SerializeField] CanvasGroup addScorecanvasGroup;
    [SerializeField] TextMeshProUGUI addScoreCounter;
    [SerializeField] PopupWindow popup;
    [SerializeField] GameObject mark;
    [SerializeField] GameObject eventTimer, cooldownTimer;
    int curScore;

    Coroutine coroutine;

    void Awake()
    {
        curScore = Mathf.Max(GameData.Saves.SkyAdventureLvl, 0);
        slider.value = (float)curScore / 21;
    }

    void OnEnable()
    {
        coroutine = StartCoroutine(UpdateTimer());

        slider.gameObject.SetActive(GameData.Saves.SkyAdventureLvl >= 0);
        if (curScore < GameData.Saves.SkyAdventureLvl)
        {
            int diff = GameData.Saves.SkyAdventureLvl - curScore;
            curScore = GameData.Saves.SkyAdventureLvl;

            addScore.gameObject.SetActive(true);
            addScoreCounter.text = $"+{diff}";
            Vector3 startPos = addScore.localPosition;
            UIAnimation.Appear(addScorecanvasGroup, 0, 0.33f, () => addScore.DOLocalMoveX(0, 0.33f).SetDelay(0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                addScore.localPosition = startPos;
                addScore.gameObject.SetActive(false);
                slider.DOValue((float)curScore / 21, 0.33f);
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
            bool isCooldown = GameData.Saves.SkyAdventureCooldown.AddMinutes(GameData.Default.eventLoseCooldown) > DateTime.Now;
            eventTimer.SetActive(!isCooldown);
            cooldownTimer.SetActive(isCooldown);
            if (GameData.Saves.SkyAdventureLvl == -1 && !isCooldown) mark.SetActive(true);
            yield return new WaitForSeconds(1);
        }
    }

    public void Click()
    {
        if (GameData.Saves.SkyAdventureLvl == -1) popup.Show();
        else SkyAdventureWindow.Instance.AnimationShow();
    }
}