using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkyAdventureStartWindow : PopupWindow
{
    [SerializeField] GameObject[] objs;
    [SerializeField] TextMeshProUGUI playersCount;
    [SerializeField] Slider playersSlider;
    [SerializeField] GameObject playersParent;
    [SerializeField] SkyAdventurePlayer[] players;
    [SerializeField] GameObject tapToContinue;

    public override void Show()
    {
        base.Show();

        playersParent.SetActive(false);
        tapToContinue.SetActive(false);
        Tween lastTween = null;
        for (int i = 0; i < objs.Length; i++) lastTween = UIAnimation.Show(objs[i], 0.2f + i * 0.1f);
        lastTween.OnComplete(() =>
        {
            playersParent.SetActive(true);
            for (int i = 0; i < players.Length; i++)
            {
                int index = i;
                players[index].Init(SkyAdventureWindow.Instance.PlayerColors[Random.Range(0, SkyAdventureWindow.Instance.PlayerColors.Length)], SkyAdventureWindow.RandomLetter);
                lastTween = players[index].thisTransform.DOScale(0, 0.33f).From().SetDelay(index * 0.066f).OnComplete(() => UpdatePlayersCounter(index));
            }
            players[0].Init(SkyAdventureWindow.Instance.PlayerColors[Random.Range(0, SkyAdventureWindow.Instance.PlayerColors.Length)], SkyAdventureWindow.PlayerLetter);
            lastTween.OnStart(() => UIAnimation.Show(tapToContinue, 0.2f));
        });
    }

    void UpdatePlayersCounter(int player)
    {
        float value = (float)player / (players.Length - 1);
        playersCount.text = $"{(int)(value * 100)}/100";
        playersSlider.value = value;
    }

    public void Click()
    {
        if (!tapToContinue.activeSelf) return;
        Hide();
    }
}