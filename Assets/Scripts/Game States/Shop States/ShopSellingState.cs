using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class ShopSellingState : State<GameController>
{
    public static ShopSellingState i { get; private set; }

    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;


    private void Awake()
    {
        i = this;
    }

    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public List<ItemBase> AvailableItems { get; set; }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

        StartCoroutine(StartSellingState());
    }

    IEnumerator StartSellingState()
    {
        yield return gc.StateMachine.PushAndWait(InventoryState.i);
        var selectedItem = InventoryState.i.SelectedItem;
        if (selectedItem != null)
        {
            //user picked an actual item to sell
            yield return (SellItem(selectedItem)); //if they want to continue selling more items
            StartCoroutine(StartSellingState());

        }
        else
            gc.StateMachine.Pop();
    }

    IEnumerator SellItem(ItemBase itemToSell)
    {
        //first check if you can't sell item. if you cant print out sorry I cant accept that.
        if (!itemToSell.IsSellable)
        {
            yield return DIalogManager.Instance.ShowDialogText("Sorry I cant accept that Item.");
            //return state to selling so user can sell
            yield break;
        }
        //opening wallet 
        walletUI.Show();

        float sellingPrice = Mathf.Round(itemToSell.Price / 2);
        int countTosell = 1;


        int itemCount = inventory.GetItemCount(itemToSell);
        if (itemCount > 1)
        {
            yield return DIalogManager.Instance.ShowDialogText($"How many would you like to sell?",
                waitForInput: false, autoClose: false);

            yield return countSelectorUI.ShowSelector(itemCount, sellingPrice,
               (selectedCount) => countTosell = selectedCount);

            DIalogManager.Instance.CloseDialog();
        }

        sellingPrice = sellingPrice * countTosell;


        int selectedChoice = 0;
        yield return DIalogManager.Instance.ShowDialogText($"I can give you ₽{sellingPrice} for that item. Would you like to sell it?",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
           onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //yes they want to sell so logic to sell item.
            inventory.RemoveItem(itemToSell, countTosell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DIalogManager.Instance.ShowDialogText($"Turned over {itemToSell.Name}. You recieved ₽{sellingPrice}");
        }
        //closing wallet
        walletUI.Close();

    }
}
