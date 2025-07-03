using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTriggable
{
    [SerializeField] Dialog dialog;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
       StartCoroutine(DIalogManager.Instance.ShowDialog(dialog));
    }

    public bool TriggerRepeatedly => false;
}
