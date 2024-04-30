using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITransition : MonoBehaviour
{
    static UITransition Instance = null;

    [SerializeField] float duration = 0.33f;
    [SerializeField] Ease ease = Ease.OutQuad;
    [SerializeField] Image image;

    public static event Action CloseEvent;
    public static event Action OpenEvent;

    public static UITransition Instantiate()
    {
        if (Instance) return Instance;
        Instance = Instantiate(GameData.Default.transition, UIManager.Instance.canvas);
        return Instance;
    }

    public void Close(Action action = null)
    {
        gameObject.SetActive(true);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        image.DOFade(1, duration).SetEase(ease).OnComplete(() =>
        {
            // gameObject.SetActive(false);
            CloseEvent?.Invoke();
            action?.Invoke();
        });
    }

    public void Open(Action action = null)
    {
        gameObject.SetActive(true);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        image.DOFade(0, duration).SetEase(ease).OnComplete(() =>
        {
            gameObject.SetActive(false);
            OpenEvent?.Invoke();
            action?.Invoke();
        });
    }
}
