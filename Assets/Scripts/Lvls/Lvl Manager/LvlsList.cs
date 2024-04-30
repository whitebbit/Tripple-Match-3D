using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Lvls List")]
public class LvlsList : ScriptableObject
{
    // [System.Serializable]
    // public class Lvl
    // {
    //     public Level data;
    // }

    public bool randomizedLvls;
    public int loopStartLvl;
    public List<Level> lvls;
}
