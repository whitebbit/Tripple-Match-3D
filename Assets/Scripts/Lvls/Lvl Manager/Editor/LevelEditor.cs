using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    GameData gameData;
    ReorderableList list;
    SerializedProperty time, difficult;

    Editor[] gameObjectEditor;
    int[] gameObjectIndex;

    private void Awake()
    {
        gameData = AssetDatabase.LoadAssetAtPath<GameData>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Game Data", new[] { "Assets/Data" })[0]));
    }

    private void OnEnable()
    {
        int length = gameData.items.Count;

        // List<string> items = new();
        // for (int i = 0; i < length; i++) items.Add(gameData.items[i].name);
        // List<string> sortedItems = new(items);
        // sortedItems.Sort(delegate (string x, string y)
        // {
        //     return string.Compare(x, y);
        // });

        time = serializedObject.FindProperty("time");
        difficult = serializedObject.FindProperty("difficulty");

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;
        gameObjectEditor = new Editor[64];
        gameObjectIndex = new int[64];

        list = new ReorderableList(serializedObject, serializedObject.FindProperty("items"), true, true, true, true);
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty item = element.FindPropertyRelative("item");
            SerializedProperty count = element.FindPropertyRelative("count");
            SerializedProperty quest = element.FindPropertyRelative("quest");

            if (gameObjectEditor[index] == null || gameObjectIndex[index] != item.intValue)
            {
                gameObjectEditor[index] = CreateEditor(gameData.items[item.intValue].gameObject);
                gameObjectIndex[index] = item.intValue;
            }
            gameObjectEditor[index].OnInteractivePreviewGUI(new Rect(rect.x, rect.y + 15, 80, 80), bgColor);

            // item.intValue = items.IndexOf(sortedItems[EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width / 3, rect.height), sortedItems.IndexOf(items[item.intValue]), sortedItems.ToArray())]);
            // item.intValue = idItem[EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width / 3, rect.height), itemsID[item.intValue], items.ToArray())];
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), item);
            EditorGUI.IntSlider(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), count, 0, 50, GUIContent.none);
            quest.boolValue = EditorGUI.Toggle(new Rect(rect.x + rect.width / 3 + 10, rect.y - 3, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), quest.boolValue);
        };
        list.elementHeightCallback = (int index) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUIUtility.singleLineHeight * 5;
        };
        list.drawHeaderCallback = (Rect rect) =>
        {
            int itemCount = 0;
            for (int i = 0; i < list.serializedProperty.arraySize; i++) itemCount += list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("count").intValue;

            EditorGUILayout.BeginHorizontal();
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, rect.height), "Предметы на уровне");
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height), $"В сумме: {itemCount}");
            EditorGUILayout.EndHorizontal();
        };
    }

    void OnDisable()
    {
        for (int i = 0; i < gameObjectEditor.Length; i++) if (gameObjectEditor[i] != null) Editor.DestroyImmediate(gameObjectEditor[i]);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        EditorGUILayout.PropertyField(time);
        EditorGUILayout.PropertyField(difficult);
        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}