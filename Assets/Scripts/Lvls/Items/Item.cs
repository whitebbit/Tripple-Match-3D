using System;
using System.Collections;
using DG.Tweening;
using HighlightPlus;
using UnityEngine;

public class Item : MonoBehaviour
{
#if UNITY_EDITOR
    [ItemTags(0)] public int sub1Tags;
    [ItemTags(1)] public int sub2Tags;
#endif

    [NonSerialized] public int ID;
    [NonSerialized] public bool isQuest;
    [NonSerialized] public Transform thisTransform;
    [NonSerialized] public Rigidbody thisRigidbody;
    Collider thisCollider;
    HighlightEffect highlighter;

    protected Tween scaleTween;
    [NonSerialized] public Tween flyTween;
    protected Vector3 startScale;

    public event Action<Item> OnCollectEvent, OnReturnEvent;

    public void Init(int ID)
    {
        this.ID = ID;
        thisTransform = transform;
        thisRigidbody = GetComponent<Rigidbody>();
        if (TryGetComponent(out Collider collider)) thisCollider = collider; else if (thisTransform.GetChild(0).TryGetComponent(out collider)) thisCollider = collider;
        startScale = thisTransform.localScale;
    }

    public void Select()
    {        
        scaleTween.Kill();
        scaleTween = thisTransform.DOScale(startScale * 1.1f, 0.2f);

        highlighter = thisCollider.gameObject.AddComponent<HighlightEffect>();
        highlighter.ProfileLoad(GameData.Default.highlightEffectProfile);
        highlighter.outlineWidth = (float)Screen.height / Screen.width * 1.1f;
        highlighter.highlighted = true;
    }

    public void Deselect()
    {
        scaleTween.Kill();
        scaleTween = thisTransform.DOScale(startScale, 0.2f);

        if (highlighter) Destroy(highlighter);
    }

    public void SetKinematic(bool enable)
    {
        thisRigidbody.isKinematic = enable;
        thisRigidbody.useGravity = !enable;
        thisCollider.enabled = !enable;
    }

    protected void SetCollected()
    {
        Deselect();
        SetKinematic(true);

        scaleTween.Kill();
        scaleTween = thisTransform.DOScale(startScale * 0.5f, 0.33f);
        SoundHolder.Default.PlayFromSoundPack("Collect Item");
    }

    public virtual void Collect()
    {
        SetCollected();
        OnCollectEvent?.Invoke(this);
    }

    public void Return()
    {
        SetKinematic(false);
        flyTween.Kill();
        thisRigidbody.velocity += (-thisTransform.position + Vector3.up * 5) * 2;
        scaleTween.Kill();
        thisTransform.localScale = startScale;
        OnReturnEvent?.Invoke(this);
    }

    public void Fly()
    {
        flyTween = thisTransform.DOLocalMoveY(thisTransform.localPosition.y + 0.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void Destroy()
    {
        flyTween.Kill();
        scaleTween.Kill();
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        float magnitude = collision.relativeVelocity.magnitude;
        if (magnitude > 1)
        {
            AudioSource clip = SoundHolder.Default.PlayFromSoundPack("Impact");
            if (clip) clip.volume *= Mathf.Lerp(0, 1, (magnitude - 1) / 2);
        }
    }
}