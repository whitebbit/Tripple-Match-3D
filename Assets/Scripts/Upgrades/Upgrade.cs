// using UnityEngine;
// using YG;

// [CreateAssetMenu(menuName = "Scriptables/Upgrade")]
// public class Upgrade : ScriptableObject
// {
//     public UpgradeType type;

//     [System.Serializable]
//     public class Lvl
//     {
//         public float value;
//         public int price;
//     }
//     public Lvl[] lvl;

//     public float CurValue => CurLvl < 0 ? 1 : lvl[CurLvl].value;
//     public int CurPrice => lvl[CurLvl + 1].price;
//     int CurLvl => GameData.Saves.UpgradesLvl[(int)type];
//     public int MaxLvl => lvl.Length;

// #if UNITY_EDITOR
//     public string valueFormula;
//     public string priceFormula;
// #endif
// }

// public enum UpgradeType
// {
// }