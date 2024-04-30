using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : Window
{
    [SerializeField] Image background;
    [SerializeField] GameObject frame;
    [SerializeField] CanvasGroup canvasGroup;

    public override void Show()
    {
        gameObject.SetActive(true);
        UIAnimation.Appear(background);
        UIAnimation.Show(frame);
        canvasGroup.interactable = true;
    }

    public override void Hide()
    {
        canvasGroup.interactable = false;
        UIAnimation.StopAnimation(frame);
        Transform frameTransform = frame.transform;
        Vector3 startScale = frameTransform.localScale;
        frameTransform.DOScale(startScale * 0.8f, 0.2f).OnComplete(() => frameTransform.localScale = startScale);
        UIAnimation.Fade(canvasGroup, 0, 0.2f);
        UIAnimation.Fade(background, 0, 0.2f, () =>
        {
            gameObject.SetActive(false);
            base.Hide();
        });
    }
}