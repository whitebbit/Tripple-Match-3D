using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelParser : EditorWindow
{
    readonly static Dictionary<string, LevelDifficult> Difficulty = new Dictionary<string, LevelDifficult> { { "H", LevelDifficult.Hard }, { "EH", LevelDifficult.ExtraHard }, { "VH", LevelDifficult.VeryHard } };

    TextAsset itemsTxt, lvlsTxt;
    GameData gameData;
    LvlsList lvlsList;
    Dictionary<string, int> items;
    bool complete;

    [MenuItem("Tools/Lvl Parser")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelParser));
    }

    private void Awake()
    {
        gameData = AssetDatabase.LoadAssetAtPath<GameData>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Game Data", new[] { "Assets/Data" })[0]));
        lvlsList = AssetDatabase.LoadAssetAtPath<LvlsList>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Lvls List", new[] { "Assets/Data" })[0]));
    }

    void OnGUI()
    {
        itemsTxt = EditorGUILayout.ObjectField("Предметы.txt", itemsTxt, typeof(TextAsset), false) as TextAsset;
        lvlsTxt = EditorGUILayout.ObjectField("Уровни.txt", lvlsTxt, typeof(TextAsset), false) as TextAsset;
        if (GUILayout.Button("Parse")) Parse();
        if (complete) GUILayout.Label("Complete!");
    }

    void Parse()
    {
        string itemsString = itemsTxt.ToString();
        string lvlsString = lvlsTxt.ToString();
        items = new() { { "Какашка", gameData.items.FindIndex(x => x.name.Trim() == "Какашка") } };

        List<Item> gameDataItems = new List<Item>(gameData.items);
        string[] itemsLines = itemsString.Split('\n');
        for (int i = 0; i < itemsLines.Length; i++)
        {
            string[] names = itemsLines[i].Split(new char[] { '	', ' ' }, 2);
            if (names.Length != 2)
            {
                Debug.LogError($"Ошибка в {i + 1} строке    (\"{itemsLines[i]}\")");
            }
            items[names[0]] = gameDataItems.FindIndex(x => x.name.Trim() == names[1].Trim());
            if (items[names[0]] == -1) Debug.Log("Объект не найден: " + names[0] + " / " + names[1]);
        }

        string[] lvlsLines = lvlsString.Split("_У");
        for (int i = 0; i < lvlsLines.Length; i++)
        {
            string[] names = lvlsLines[i].Split(new char[] { '	', ' ', '\n' });
            if (names[0] != "")
            {
                List<Level.Items> newItems = new();
                int questCount = int.Parse(names[2].Trim());
                LevelDifficult difficulty = Difficulty.ContainsKey(names[3].Trim()) ? Difficulty[names[3].Trim()] : LevelDifficult.None;
                int start = difficulty == LevelDifficult.None ? 3 : 4;
                for (int n = 0; n < (names.Length - start) / 2; n++)
                {
                    int count = int.Parse(names[start + 1 + n * 2]);
                    string itemName = names[start + n * 2];
                    if (!items.ContainsKey(itemName)) Debug.Log("Объект не найден: " + itemName);
                    itemName = items.ContainsKey(itemName) ? itemName : "Какашка";
                    int item = items[itemName];
                    if (item < 0 || item >= gameDataItems.Count)
                    {
                        item = items["Какашка"];
                        Debug.Log("Объект не найден: " + itemName);
                    }
                    newItems.Add(new Level.Items() { item = item, count = count, quest = questCount > n });
                }

                Level level = lvlsList.lvls[int.Parse(names[0].Trim()) - 1];
                EditorUtility.SetDirty(level);
                level.items = newItems;
                level.time = int.Parse(names[1].Trim());
                level.difficulty = difficulty;
            }
        }

        complete = true;
    }
}
