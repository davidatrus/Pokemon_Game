using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;

    public ItemBase SelectedItem { get; private set; }
    public static InventoryState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    Inventory inventory;

    GameController gc;

    private void Start()
    {
       inventory = Inventory.GetInventory();
    }
    public override void Enter(GameController owner)
    {
        gc = owner;

        SelectedItem = null;

        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        inventoryUI.HandleUpdate();
    }

    public override void Exit()
    {
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
    }
    void OnItemSelected(int selection)
    {
        SelectedItem = inventoryUI.SelectedItem;

        if (gc.StateMachine.GetPreviousState() != ShopSellingState.i)
            StartCoroutine(SelectPokemonAndUseItem());
        else
            gc.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedItem = null;
        gc.StateMachine.Pop();
    }

    IEnumerator SelectPokemonAndUseItem()
    {
        var prevState = gc.StateMachine.GetPreviousState();
        if (prevState == BattleState.i)
        {
            //in battle so only allow items that can be used in battle
            if (!SelectedItem.CanUseInBattle)
            {
                yield return DIalogManager.Instance.ShowDialogText("This Item can't be used in battle!");
                yield break;
            }
        }
        else
        {
            //outside of battle so only use items that can only be used outside of battle
            if (!SelectedItem.CanUseOustideBattle)
            {
                yield return DIalogManager.Instance.ShowDialogText("This Item can't be used outside battle!");
                yield break;
            }

        }


        if (SelectedItem is PokeballItems)
        {
            inventory.UseItem(SelectedItem, null); //if pokemon use the selected pokeball and the pokemon to use on is null 
            gc.StateMachine.Pop();
            yield break;
        }
       yield return gc.StateMachine.PushAndWait(PartyState.i);

        if(prevState == BattleState.i)
        {
            if (UseItemState.i.ItemUsed)
                gc.StateMachine.Pop();
        }
    }

}
