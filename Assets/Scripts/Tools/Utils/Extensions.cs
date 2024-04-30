using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T Random<T>(this IList<T> list)
    {
        if (list.Count == 0)
            return default;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static List<T> Random<T>(this List<T> list, int count)
    {
        if (list.Count == 0)
            return default;
        List<T> random = new(list);
        for (int i = 0; i < list.Count - count; i++) random.RemoveAt(UnityEngine.Random.Range(0, random.Count));
        return random;
    }

    public static float Random(this Vector2 vec)
    {
        return UnityEngine.Random.Range(vec.x, vec.y);
    }
}