using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//class just for cleaning up the cutscene editor in the inspector using propertyDrawer 
[CustomPropertyDrawer(typeof(CutsceneActor))]

public class CutScenceActorDrawer : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
   {
        EditorGUI.BeginProperty(position, label, property);
      position =  EditorGUI.PrefixLabel(position, label);
      var togglePosition = new Rect(position.x, position.y, 70, position.height);
      var fieldPosition = new Rect(position.x + 70, position.y, position.width - 70, position.height);

      var isPlayerProperty = property.FindPropertyRelative("isPlayer");
      isPlayerProperty.boolValue = GUI.Toggle(togglePosition, isPlayerProperty.boolValue, "Is Player");
      isPlayerProperty.serializedObject.ApplyModifiedProperties();

      if(!isPlayerProperty.boolValue)
          EditorGUI.PropertyField(fieldPosition, property.FindPropertyRelative("character"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
