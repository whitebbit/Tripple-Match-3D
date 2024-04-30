using System;
using UnityEngine;

public class CarRacePlayer : MonoBehaviour
{
    [NonSerialized] public Transform thisTransform;

    public void Init()
    {
        thisTransform = transform;
    }
}
