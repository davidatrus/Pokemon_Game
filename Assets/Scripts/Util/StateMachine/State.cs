using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKMNGAMEUtils.StateMachine
{
    public class State<T> : MonoBehaviour
    {
        //virtual so subclasses can override 
        public virtual void Enter(T owner) { }

        //Update function of state machine 
        public virtual void Execute() { }

        //exectued when we exit a state
        public virtual void Exit() { }

    }
}