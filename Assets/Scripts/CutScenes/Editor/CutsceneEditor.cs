using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //converting target to a cutscene 
        var cutscene = target as Cutscene;

        using (var scope = new GUILayout.HorizontalScope()) {

            if (GUILayout.Button("Dialog"))
                cutscene.AddAction(new DialogAction());
            else if (GUILayout.Button("Move"))
                cutscene.AddAction(new MoveAction());
            else if (GUILayout.Button("Turn"))
                cutscene.AddAction(new TurnAction());
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Teleport Obj"))
                cutscene.AddAction(new TeleportObjectAction());
            else if (GUILayout.Button("Enable Obj"))
                cutscene.AddAction(new EnableObjectAction());
            else if (GUILayout.Button("Disable Obj"))
                cutscene.AddAction(new DisableObjectAction());
        }
        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("NPC Interact"))
                cutscene.AddAction(new NPCInteractAction());
            else if (GUILayout.Button("Fade In"))
                cutscene.AddAction(new FadeInAction());
            else if (GUILayout.Button("Fade Out"))
                cutscene.AddAction(new FadeOutAction());
        }


        base.OnInspectorGUI();

    }
}
