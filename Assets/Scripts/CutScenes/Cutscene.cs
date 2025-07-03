using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cutscene : MonoBehaviour, IPlayerTriggable
{
    //serialzing as a reference instead of a value. 
    [SerializeReference]
    [SerializeField] List<CutSceneAction> actions;

    public bool TriggerRepeatedly => false;

    public IEnumerator Play()
    {
        GameController.Instance.StateMachine.Push(CutSceneState.i);
        foreach(var action in actions)
        {
            if (action.ShouldWeWait)
                yield return action.Play();
            else
                StartCoroutine(action.Play());
        }

        GameController.Instance.StateMachine.Pop();
    }

    //dynamically load the actions into cutscene actions list
    public void AddAction(CutSceneAction action)
    {
        //making changes on editor tell unity that scene has been modified and will prompt you to save before switching scenes. and allow using ctrl z to properly work
        //conditional compilation
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Add action to cutscne");
#endif
        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Play());
    }
}
