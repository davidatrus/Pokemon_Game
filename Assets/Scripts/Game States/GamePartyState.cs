using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class PartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public Pokemon SelectedPokemon { get; private set; }

    public static PartyState i { get; private set; }

    private void Awake()
    {
        i = this;
    }
    GameController gc;
    //on the enter execute and exit funcs  just setting gc instance as owner, setting game object for whatever screen to be active
    //then subscribing to the onselected actions and on back actions they gain, execute the handleupdate then in exit disable game object and unsubscribe from the events.
    public override void Enter(GameController owner)
    {
        gc = owner;
        SelectedPokemon = null;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;

    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    void OnPokemonSelected(int selection)
    { 
        SelectedPokemon = partyScreen.SelectedMember;
        StartCoroutine(PokemonSelectedAction(selection));

    }

    IEnumerator PokemonSelectedAction(int selectedPokemonIndex)
    {
       

        var prevState = gc.StateMachine.GetPreviousState();

        var battleState = prevState as BattleState;

        //if coming from inventory state use item 
        if (prevState == InventoryState.i)
        {
            StartCoroutine(GoToUseItemState());
        }
        else if (prevState == BattleState.i)
        {
            DynamicMenuState.i.MenuItems = new List<string>() { "Shift", "Summary", "Cancel" };
            yield return gc.StateMachine.PushAndWait(DynamicMenuState.i);
            if (DynamicMenuState.i.SelectedItem == 0)
            {
                //user selected shift
                if (SelectedPokemon.HP <= 0)
                {
                    partyScreen.SetMessageText("You cant send out a fainted pokemon.");
                    yield break;
                }
                if (SelectedPokemon == battleState.BattleSystem.PlayerUnit.Pokemon)
                {
                    partyScreen.SetMessageText("You cant send out the same pokemon...");
                    yield break;
                }
                gc.StateMachine.Pop();

            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                //Summary selected
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gc.StateMachine.PushAndWait(SummaryState.i);
            }
            else
            {
                yield break;
            }

        }
        else
        {
            DynamicMenuState.i.MenuItems = new List<string>() { "Summary", "Switch Position", "Cancel" };
            yield return gc.StateMachine.PushAndWait(DynamicMenuState.i);
            if(DynamicMenuState.i.SelectedItem == 0)
            {
                //user selected summary
                //SUMMARY SCREEN
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gc.StateMachine.PushAndWait(SummaryState.i);
            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                //Switch position selected
            }
            else
            {
                yield break;
            }
            
        }
    }

    IEnumerator GoToUseItemState()
    {
       yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedPokemon = null;

        var prevState = gc.StateMachine.GetPreviousState();
        if(prevState== BattleState.i)
        {
           var battleState =  prevState as BattleState;
            if (battleState.BattleSystem.PlayerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a pokemon to continue!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
        }

        gc.StateMachine.Pop();
    }
}
