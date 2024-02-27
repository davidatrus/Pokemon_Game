using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//editor tool for map area, just checks to make sure % chance for pokemon is = 100

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int totalChanceGrass = serializedObject.FindProperty("totalChance").intValue;
        int totalChanceWater = serializedObject.FindProperty("totalChanceWater").intValue;

        if(totalChanceGrass!=100 && totalChanceGrass!=-1)
            EditorGUILayout.HelpBox($"The total Chance in grass is {totalChanceGrass} which is not equal to 100.", MessageType.Error);
        if (totalChanceWater != 100 && totalChanceWater!=-1)
            EditorGUILayout.HelpBox($"The total Chance in water is {totalChanceWater} which is not equal to 100.", MessageType.Error);
    }

}
