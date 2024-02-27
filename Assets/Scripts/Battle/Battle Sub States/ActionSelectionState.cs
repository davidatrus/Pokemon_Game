using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSectionUI selectionUI;

    BattleSystem bs;
    public static  ActionSelectionState i {get; private set;}

    private void Awake()
    {
        i = this;
    }

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnActionSelected;
        bs.DialogBox.SetDialog("Choose an action");
    }


    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }
    public override void Exit()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnActionSelected;
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0) //fight action
        {
            bs.SelectedAction = BattleAction.Move; 
            MoveSelectionState.i.Moves = bs.PlayerUnit.Pokemon.Moves;
            bs.StateMachine.ChangeState(MoveSelectionState.i);
        }
        else if(selection == 1) //bag
        {
            StartCoroutine(GoToInventoryState());
        }
        else if (selection == 2) // pokemon
        {
            StartCoroutine(GoToPartyState()); 
        }
        else if (selection == 3) //Run selected
        {
            bs.SelectedAction = BattleAction.Run;
            bs.StateMachine.ChangeState(RunTurnState.i);
        }
    }


    IEnumerator GoToPartyState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
       var selectedPokemon = PartyState.i.SelectedPokemon;
        if(selectedPokemon != null)
        {
            bs.SelectedAction = BattleAction.SwitchPokemon;
            bs.SelectedPokemon = selectedPokemon;
            bs.StateMachine.ChangeState(RunTurnState.i);
        }
    }

    IEnumerator GoToInventoryState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(InventoryState.i);
        var selectedItem = InventoryState.i.SelectedItem;
        if(selectedItem!= null ) //item was used
        {
            bs.SelectedAction = BattleAction.UseItem;
            bs.SelectedItem = selectedItem;
            bs.StateMachine.ChangeState(RunTurnState.i);
        }
    }

}
