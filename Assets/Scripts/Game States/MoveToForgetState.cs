using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class MoveToForgetState : State<GameController> 
{
    [SerializeField] MoveToForgetSelectionUI moveSelectionUI;

// inptus that state req 
    public List<MoveBase> CurrentMoves { get; set; }
    public MoveBase NewMove { get; set; }

    //output of state
    public int Selection { get; set; }

    public static MoveToForgetState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        Selection = 0;

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(CurrentMoves, NewMove);

        moveSelectionUI.OnSelected += OnMoveSelected;
        moveSelectionUI.OnBack += OnBack;

    }
    public override void Execute()
    {
        moveSelectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        moveSelectionUI.gameObject.SetActive(false);
        moveSelectionUI.OnSelected -= OnMoveSelected;
        moveSelectionUI.OnBack -= OnBack;

    }

    void OnMoveSelected(int selection)
    {
        //expose the selction for the useItemState
        Selection = selection;
        gc.StateMachine.Pop();
    }

    void OnBack()
    {
        Selection = -1; //selection wasnt made 
        gc.StateMachine.Pop();
    }
}
