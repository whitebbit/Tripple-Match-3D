using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICarRaceRewardList : MonoBehaviour
{
    List<GameObject> rewards;
    Transform rewardsTransform;

    void Awake()
    {
        rewards = new();
        rewardsTransform = transform;
    }

    void OnEnable()
    {
        int lvl = Mathf.Clamp(GameData.Saves.CarRaceLvl, 0, GameData.Default.carRaceLvls.Length - 1);
        for (int i = 0; i < GameData.Default.carRaceLvls[lvl].rewards.Length; i++)
        {
            rewards.Add(GameData.Default.carRaceLvls[lvl].rewards[i].Instantiate(rewardsTransform).gameObject);
        }
    }

    void OnDisable()
    {
        rewards.ForEach(x => Destroy(x));
        rewards.Clear();
    }
}