using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCustomItemsTransform : MonoBehaviour
{
    [SerializeField] Transform[] points;

    void OnEnable()
    {
        Level.OnChangeState += SetPositions;
    }

    void OnDisable()
    {
        Level.OnChangeState -= SetPositions;
    }

    void SetPositions(LevelState state)
    {
        if (state == LevelState.Process)
        {
            ref List<Item> items = ref Level.Instance.curItems;
            for (int i = 0; i < points.Length; i++)
            {
                items[i].thisTransform.position = points[i].position;
                items[i].thisTransform.rotation = points[i].rotation;
            }
        }
    }
}
