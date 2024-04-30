using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuicyTestItemCounter : MonoBehaviour
{
    #region Singletone
    private static JuicyTestItemCounter _instance;
    public static JuicyTestItemCounter Instance { get => _instance; }
    public JuicyTestItemCounter() => _instance = this;
    #endregion

    [SerializeField] GameObject counter;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] Transform txtTransform;
    [SerializeField] ParticleSystem particles;
    Transform thisTransform;
    [NonSerialized] public int itemCount;
    Tween txtTransformTween;

    void Awake()
    {
        thisTransform = transform;
    }

    void OnEnable()
    {
        if (GameData.Saves.JuicyTestCurType >= 0)
        {
            itemCount = 0;
            txt.text = itemCount.ToString();
            counter.SetActive(false);
            icon.sprite = GameData.Default.juicyTestData.types[GameData.Saves.JuicyTestCurType].icon;

            Level.OnWinEvent += AddScore;
        }
    }

    void OnDisable()
    {
        if (GameData.Saves.JuicyTestCurType >= 0)
        {
            counter.SetActive(false);
            Level.OnWinEvent -= AddScore;
        }
    }

    public void CollectItem(Item item)
    {
        counter.SetActive(true);
        particles.Play();
        itemCount++;
        txt.text = itemCount > 0 ? itemCount.ToString() : "";
        txtTransformTween.Kill(true);
        txtTransformTween = txtTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        item.Destroy();
    }

    void AddScore(int score) => UIJuicyTestSlider.Instance.AddScore(itemCount);
}