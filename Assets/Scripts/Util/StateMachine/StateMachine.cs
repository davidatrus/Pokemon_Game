using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//state machine to better handle gameplay state logic, was prev only storing current and last state.

namespace PKMNGAMEUtils.StateMachine
{


    public class StateMachine<T>
    {

        public State<T> CurrentState { get; private set; }

        public Stack<State<T>> StateStack { get; private set; }

        T owner;

        public StateMachine(T owner)
        {
            this.owner = owner;
            StateStack = new Stack<State<T>>();
        }

        public void Exectue()
        {
            //null condition just to make sure if by any chance state is null game will still run
            CurrentState?.Execute();
        }

        public void Push(State<T> newState)
        {
            StateStack.Push(newState);
            //current state always state at top of the stack
            CurrentState = newState;
            CurrentState.Enter(owner);
        }

        public void Pop()
        {
            StateStack.Pop();
            CurrentState.Exit();
            CurrentState = StateStack.Peek();
        }

        //replace a state at top of stack so we dont always have to  use push,pop func's
        public void ChangeState(State<T> newState)
        {
            if(CurrentState!=null)
            {
                StateStack.Pop();
                CurrentState.Exit();

            }
            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.Enter(owner);

        }

        public  IEnumerator PushAndWait(State<T> newState)
        {
            //function to push and wait for current state to be old state, used when using items 
            var oldState = CurrentState;
            Push(newState);
            yield return new WaitUntil(() => CurrentState == oldState);
        }

        public State<T> GetPreviousState()
        {
            //current state is at ele 0 in stack so prev state is at 1
            return StateStack.ElementAt(1);
        }

    }
}