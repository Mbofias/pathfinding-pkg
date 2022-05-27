using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinder;

[CustomPropertyDrawer(typeof(Pathfinding))]
public class PathfindingEditor : PropertyDrawer
{
    private readonly string[] dropdownOptions = { "Multiple target points", "Single target point" };

    private GUIStyle dropdown;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (dropdown == null)
        {
            dropdown = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            dropdown.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        SerializedProperty multiplePoints = property.FindPropertyRelative("multiplePoints");
        SerializedProperty targetPoint = property.FindPropertyRelative("targetPoint");
        SerializedProperty targetPoints = property.FindPropertyRelative("targetPoints");
        SerializedProperty movement = property.FindPropertyRelative("defaultMovement");
        SerializedProperty map = property.FindPropertyRelative("map");

        Rect buttonRect = new Rect(position);
        buttonRect.width = dropdown.fixedWidth;

        position.xMin = buttonRect.xMax + 15;
        position.height = 18;

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.PropertyField(position, map, GUIContent.none);

        position.y += 20;

        EditorGUI.PropertyField(position, movement, GUIContent.none);

        position.y += 20;
        buttonRect.width = dropdown.fixedWidth;
        position.xMin = buttonRect.xMax + 15;

        buttonRect.yMin += dropdown.margin.top;

        int result = EditorGUI.Popup(buttonRect, multiplePoints.boolValue ? 0 : 1, dropdownOptions, dropdown);

        multiplePoints.boolValue = result == 0;

        EditorGUI.PropertyField(position,
            multiplePoints.boolValue ? targetPoints : targetPoint,
            GUIContent.none);

        if (!multiplePoints.boolValue) targetPoints.isExpanded = false;

        SetArraySize(targetPoints, position);

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty targetPoints = property.FindPropertyRelative("targetPoints");

        if (targetPoints.isExpanded)
            return (targetPoints.arraySize + 1) * 20 + 70;

        return base.GetPropertyHeight(property, label) + 40;
    }

    private void SetArraySize(SerializedProperty parent, Rect pos)
    {
        pos.xMin += 5;
        pos.xMax -= 5;
        pos.y += 5;
        if (parent.isExpanded)
            for (int i = 0; i < parent.arraySize; i++)
            {
                pos.y += 20;
                EditorGUI.PropertyField(pos, parent.GetArrayElementAtIndex(i), GUIContent.none);
            }
    }
}
