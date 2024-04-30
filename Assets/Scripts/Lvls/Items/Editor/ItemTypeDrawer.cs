using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemTypeAttribute))]
public class ItemTypeDrawer : PropertyDrawer
{
    GameData gameData;
    List<string> items;
    Dictionary<int, int> itemsID, idItem;

    public ItemTypeDrawer() : base()
    {
        gameData = AssetDatabase.LoadAssetAtPath<GameData>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Game Data", new[] { "Assets/Data" })[0]));

        string[] sub1Tags = gameData.itemTagsSub1;
        string[] sub2Tags = gameData.itemTagsSub2;

        int length = gameData.items.Count;
        items = new();
        itemsID = new();
        idItem = new();
        for (int i = 0; i < length; i++)
        {
            List<string> sub1 = new(), sub2 = new();
            for (int j = 0; j < sub1Tags.Length; j++) if (((1 << j) & gameData.items[i].sub1Tags) > 0) sub1.Add(sub1Tags[j]);
            for (int j = 0; j < sub2Tags.Length; j++) if (((1 << j) & gameData.items[i].sub2Tags) > 0) sub2.Add(sub2Tags[j]);
            if (sub2.Count > 0)
            {
                for (int j = 0; j < sub1.Count; j++)
                    for (int n = 0; n < sub2.Count; n++)
                    {
                        string name = $"{sub1[j]}/{sub2[n]}/";
                        itemsID[i] = items.Count;
                        idItem[items.Count] = i;
                        items.Add(name + gameData.items[i].name);
                    }
            }
            else
            {
                for (int j = 0; j < sub1.Count; j++)
                {
                    string name = $"{sub1[j]}/";
                    itemsID[i] = items.Count;
                    idItem[items.Count] = i;
                    items.Add(name + gameData.items[i].name);
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (itemsID.ContainsKey(property.intValue)) property.intValue = idItem[EditorGUI.Popup(position, itemsID[property.intValue], items.ToArray())];
        else
        {
            EditorGUI.BeginChangeCheck();
            int value = EditorGUI.Popup(position, -1, items.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = value;
            }
        }
        EditorGUI.EndProperty();
    }

}