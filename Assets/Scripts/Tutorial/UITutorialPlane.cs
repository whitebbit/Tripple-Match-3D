using DG.Tweening;
using UnityEngine;

public class UITutorialPlane : MonoBehaviour
{
    [SerializeField] CanvasGroup plane;
    [SerializeField] bool autoHide = true;
    protected bool canHide;

    void Awake() => GetComponent<Canvas>().worldCamera = Camera.main;

    void OnEnable()
    {
        Level.OnChangeState += ChangeState;
        plane.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        Level.OnChangeState -= ChangeState;
    }

    void ChangeState(LevelState state)
    {
        if (state == LevelState.Process) Show();
    }

    protected virtual void Show()
    {
        plane.gameObject.SetActive(true);
        DOTween.Sequence().SetDelay(0.33f).OnComplete(() => canHide = true);
    }

    void Update()
    {
        if (autoHide && canHide && Input.GetMouseButtonDown(0)) Hide();
    }

    public void Hide()
    {
        canHide = false;
        UIAnimation.Fade(plane, action: () => gameObject.SetActive(false));
    }
}