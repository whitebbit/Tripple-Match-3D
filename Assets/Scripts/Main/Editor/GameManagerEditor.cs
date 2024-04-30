using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private GameManager _gameManager;
    int cash = 1000;
    int coins = 1000;

    private void OnEnable()
    {
        _gameManager = target as GameManager;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (_gameManager.TryGetComponent(out LevelManager levelManager)) return;
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add HP", GUILayout.Width(200), GUILayout.Height(20))) GameData.Default.AddHP(cash);
        cash = EditorGUILayout.IntField(cash, GUILayout.Width(100), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Coins", GUILayout.Width(200), GUILayout.Height(20))) GameData.Default.AddCoins(coins);
        coins = EditorGUILayout.IntField(coins, GUILayout.Width(100), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Delete Saves", GUILayout.Width(200), GUILayout.Height(20)))
        {
            PlayerPrefs.DeleteAll();
            File.Delete(Path.Combine(Application.persistentDataPath, "GameData.json"));
            if (File.Exists(Path.Combine(Application.dataPath, @"YandexGame\WorkingData\Editor\SavesEditorYG.json"))) File.Delete(Path.Combine(Application.dataPath, @"YandexGame\WorkingData\Editor\SavesEditorYG.json"));
        }
    }

}