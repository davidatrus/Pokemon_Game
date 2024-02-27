using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;
using System.Linq;

public class UseItemState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    public bool ItemUsed { get; private set; }

    public static UseItemState i { get; private set; }
    Inventory inventory;

    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

        ItemUsed = false;
        StartCoroutine(UseItem());
    }

    IEnumerator UseItem()
    {
        var item = inventoryUI.SelectedItem;
        var pokemon = partyScreen.SelectedMember;

        if (item is TMItem)
        {
            yield return HandleTmItems();
        }
        else
        {
            //get item then check if it is evolution item
            if (item is EvolutionItem)
            {
                var evolution = pokemon.CheckForEvolution(item);
                if (evolution != null) //if item not equal to null means item can be used to evole pokemon
                {
                    yield return EvolutionState.i.Evolve(pokemon, evolution);
                }
                else
                {
                    yield return DIalogManager.Instance.ShowDialogText($"It won't have any affect.");
                    gc.StateMachine.Pop();
                    yield break;
                    //if we cant use the item to evole pokemon close the party screen 
                }
            }

            var usedItem = inventory.UseItem(item, partyScreen.SelectedMember);
            if (usedItem != null)
            {
                ItemUsed = true;
                if (usedItem is RecoveryItems)
                    yield return DIalogManager.Instance.ShowDialogText($"You Used a {usedItem.Name}");

            }
            else
            {
                if (inventoryUI.SelectedCategory == (int)ItemCategory.Items)
                    yield return DIalogManager.Instance.ShowDialogText($"It won't have any affect.");
            }
        }

        gc.StateMachine.Pop();

    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventoryUI.SelectedItem as TMItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.Name}!");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}!");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name}");
        }
        else
        {
            yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying to learn {tmItem.Move.Name}");
            yield return DIalogManager.Instance.ShowDialogText($"But it can not learn more than {PokemonBase.MaxNumOfMoves} moves.");

            yield return DIalogManager.Instance.ShowDialogText($"Choose a move you want to forget.", true, false);

            MoveToForgetState.i.NewMove = tmItem.Move;
            MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
            yield return gc.StateMachine.PushAndWait(MoveToForgetState.i);

           int moveIndex = MoveToForgetState.i.Selection;
            if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex==-1)
            {
                //dont learn new move
                yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {tmItem.Move.Name}.");
            }
            else
            {
                //learn the new move by forgeting selected move.
                var selectedMove = pokemon.Moves[moveIndex].Base;
                yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {tmItem.Move.Name}.");

                pokemon.Moves[moveIndex] = new Move(tmItem.Move);
            }

        }
    }
}
