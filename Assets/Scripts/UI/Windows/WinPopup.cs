using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : EndPopup
{
    [SerializeField] GameObject[] stars;
    [SerializeField] GameObject[] starsParticles;
    [SerializeField] TextMeshProUGUI timer;

    public static event Action NextLvlEvent;

    public void Show(int stars)
    {
        SoundHolder.Default.PlayFromSoundPack("Win");

        int time = (int)Level.Instance.time;
        timer.text = string.Format("{0:0}:{1:00}", time / 60, time % 60);
        for (int i = 0; i < stars; i++)
        {
            GameObject star = this.stars[i];
            GameObject particles = starsParticles[i];
            star.transform.DOScale(1.25f, 0.33f).From().SetDelay(0.33f + i * 0.15f).OnStart(() =>
            {
                star.GetComponent<Image>().DOFade(0, 0.33f).From();
                star.SetActive(true);
                particles.SetActive(true);
            });
        }

        Show();
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
            starsParticles[i].SetActive(false);
        }
    }

    public void NextLvl() => NextLvlEvent?.Invoke();
}