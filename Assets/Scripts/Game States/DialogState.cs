using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class DialogState : State<GameController>
{
   public static DialogState i { get;  private set; }

    private void Awake()
    {
        i = this;
    }
}
