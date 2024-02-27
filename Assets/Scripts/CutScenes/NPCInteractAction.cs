using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractAction : CutSceneAction
{
    [SerializeField] NPCController npc;

    public override IEnumerator Play()
    {
     yield return  npc.Interact(PlayerController.i.transform);
    }
}
