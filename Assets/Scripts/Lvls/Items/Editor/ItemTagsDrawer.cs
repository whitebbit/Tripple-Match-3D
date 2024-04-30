using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemTagsAttribute), true)]
public class ItemTagsDrawer : PropertyDrawer
{
    GameData gameData;

    public ItemTagsDrawer() : base()
    {
        gameData = AssetDatabase.LoadAssetAtPath<GameData>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Game Data", new[] { "Assets/Data" })[0]));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects) return;
        string[] tags = (attribute as ItemTagsAttribute).sub == 0 ? gameData.itemTagsSub1 : gameData.itemTagsSub2;
        property.intValue = EditorGUI.MaskField(position, label.text, property.intValue, tags);
    }

}