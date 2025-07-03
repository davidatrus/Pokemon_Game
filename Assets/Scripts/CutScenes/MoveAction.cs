using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to move characters in cutscene
[System.Serializable]
public class MoveAction : CutSceneAction
{
    [SerializeField] CutsceneActor actor;
    [SerializeField] List<Vector2> movePattern;

    public override IEnumerator Play()
    {
        var character = actor.GetCharacter();

        foreach (var moveVec in movePattern)
        {
            //normally move function wont exectue move if there is a collision so add an param so that for cutsences that require npc moving player full animation plays out
            yield return character.Move(moveVec,checkCollisions:false);
        }
    }
}

[System.Serializable]
public class CutsceneActor
{
    [SerializeField] bool isPlayer;
    [SerializeField] Character character;

    //if player character if not use the one in the field
    public Character GetCharacter() => (isPlayer) ? PlayerController.i.Character : character;
}