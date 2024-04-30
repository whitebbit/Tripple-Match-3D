using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [NonSerialized] public int ID;
    [NonSerialized] public int count;

    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI counter;
    Animator animator;
    Tween counterScaleTween;
    Transform counterTransform;

    public event Action<Quest> OnCompleteEvent;

    void OnEnable()
    {
        GameManager.OnRestartEvent += Destroy;
    }

    void OnDisable()
    {
        GameManager.OnRestartEvent -= Destroy;
    }

    public void Init(int ID, Sprite icon, int count, int order)
    {
        counterTransform = counter.transform;
        animator = GetComponent<Animator>();

        this.ID = ID;
        this.icon.sprite = icon;
        this.count = count;
        counter.text = count.ToString();

        DOTween.Sequence().SetDelay(0.3f + order * 0.1f).OnComplete(() => animator.SetBool("Open", true));
    }

    public void Reduce()
    {
        count--;
        if (count == 0) Complete();
        else UpdateCounter();
    }

    public void Return()
    {
        count++;
        UpdateCounter();
    }

    void UpdateCounter()
    {
        counter.text = count.ToString();
        counterScaleTween.Kill(true);
        counterScaleTween = counterTransform.DOPunchScale(Vector3.one * 0.15f, 0.2f);
    }

    public void ShowCounter()
    {
        counterScaleTween.Kill(true);
        counterScaleTween = counterTransform.DOScale(1, 0.33f).SetEase(Ease.OutBack);
    }

    void Complete()
    {
        GetComponent<Animator>().SetBool("Open", false);
        counter.gameObject.SetActive(false);
        OnCompleteEvent?.Invoke(this);
        transform.DOScale(0, 0.33f).SetDelay(0.33f).OnComplete(() => Destroy(gameObject));
    }

    void Destroy()
    {
        counterScaleTween.Kill();
        Destroy(gameObject);
    }
}