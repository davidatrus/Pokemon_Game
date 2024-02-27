using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class MoveSelectionState : State<BattleSystem>
{
    [SerializeField] MoveSelectionUI selectionUI;
    [SerializeField] GameObject moveDetailsUI;

    public List<Move> Moves { get; set; }


    BattleSystem bs;

    public static MoveSelectionState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        selectionUI.SetMoves(Moves);

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnMoveSelected;
        selectionUI.OnBack += OnBack;

        //moves dialog is ontop of default dialogbox so enable it and disable dialog
        moveDetailsUI.SetActive(true);
        bs.DialogBox.EnableDialogText(false);
    }

    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }


    public override void Exit()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnMoveSelected;
        selectionUI.OnBack -= OnBack;

        selectionUI.ClearItems();

        moveDetailsUI.SetActive(false);
        bs.DialogBox.EnableDialogText(true);
    }
    void OnMoveSelected(int selection)
    {
        bs.SelectedMove = selection;
        bs.StateMachine.ChangeState(RunTurnState.i);
    }

    void OnBack()
    {
        bs.StateMachine.ChangeState(ActionSelectionState.i);

    }
}
