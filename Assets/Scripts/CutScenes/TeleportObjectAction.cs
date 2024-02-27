using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObjectAction : CutSceneAction
{
    [SerializeField] GameObject tele;
    [SerializeField] Vector2 position;


    public override IEnumerator Play()
    {
        tele.transform.position = position;
        yield break;
    }
}
