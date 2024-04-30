using System;
using UnityEngine;
using UnityEngine.Localization.Components;

public class RewardIcon : MonoBehaviour
{
    public LocalizeStringEvent counter;
    [NonSerialized] public Transform thisTransform;

    protected virtual void Awake()
    {
        thisTransform = transform;
    }

    public void Init(int count)
    {
        counter.StringReference.Arguments = new object[]{ count };
        counter.RefreshString();
    }
}