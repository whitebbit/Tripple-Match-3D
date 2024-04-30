using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    private SerializedProperty _editorMode, _lvlList;
    private LevelManager _levelManager;
    int cash = 1000;
    int coins = 1000;

    private void Awake()
    {
        _levelManager = target as LevelManager;
    }

    private void OnEnable()
    {
        _editorMode = serializedObject.FindProperty("editorMode");
        _lvlList = serializedObject.FindProperty("levels");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

        _editorMode.boolValue = GUILayout.Toggle(_editorMode.boolValue, new GUIContent("Editor Mode"), GUILayout.Width(100), GUILayout.Height(20));
        _levelManager.editorMode = _editorMode.boolValue;
        if (_editorMode.boolValue)
        {
            DrawSelectedLevel();
        }

        EditorGUILayout.PropertyField(_lvlList);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add HP", GUILayout.Width(200), GUILayout.Height(20))) GameData.Default.AddHP(cash);
        cash = EditorGUILayout.IntField(cash, GUILayout.Width(100), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Coins", GUILayout.Width(200), GUILayout.Height(20))) GameData.Default.AddCoins(coins);
        coins = EditorGUILayout.IntField(coins, GUILayout.Width(100), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Give All Ability", GUILayout.Width(200), GUILayout.Height(20)))
        {
            for (int i = 0; i < GameData.Saves.AbilityCount.Length; i++) GameData.Saves.AbilityCount[i]++;
            GameData.SaveProgress();
        }

        if (GUILayout.Button("Delete Saves", GUILayout.Width(200), GUILayout.Height(20)))
        {
            PlayerPrefs.DeleteAll();
            File.Delete(Path.Combine(Application.persistentDataPath, "GameData.json"));
            if (File.Exists(Path.Combine(Application.dataPath, @"YandexGame\WorkingData\Editor\SavesEditorYG.json"))) File.Delete(Path.Combine(Application.dataPath, @"YandexGame\WorkingData\Editor\SavesEditorYG.json"));
        }
    }

    private void DrawSelectedLevel()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        int index = EditorGUILayout.IntField("Current Level", _levelManager.CurrentLevelIndex + 1);
        if (EditorGUI.EndChangeCheck())
        {
            _levelManager.SelectLevel(index - 1);
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("<<", GUILayout.Width(30), GUILayout.Height(20)))
        {
            _levelManager.PrevLevel();
        }
        if (GUILayout.Button(">>", GUILayout.Width(30), GUILayout.Height(20)))
        {
            _levelManager.NextLevel();
        }

        EditorGUILayout.EndHorizontal();
    }

}