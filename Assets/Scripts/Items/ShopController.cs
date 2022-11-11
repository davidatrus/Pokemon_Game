using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ShopState { Menu, Buying, Selling, Busy} 

public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] Vector2 shopCameraOffset;

    public event Action OnStart;
    public event Action OnFinish;


    ShopState state;

    Merchant merchant;

    //making it a singleton to acess it from Merchant.cs file
    public static ShopController i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory(); // getting instance of inventory 
    }
    //script for all our logic to buy and sell items

    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;
        OnStart?.Invoke();
       yield return StartMenuState();

    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;
        yield return DIalogManager.Instance.ShowDialogText("Welcome! What do you need?",
            waitForInput: false,
            choices: new List<string>() { "Buy", "Sell", "Quit" },
           onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //user clicked buy so buying items logic
            yield return GameController.Instance.MoveCamera(shopCameraOffset);
            walletUI.Show();
            shopUI.ShowUI(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item)),
               () => StartCoroutine(OnBackFromBuying()));

            state = ShopState.Buying;
        }
        else if (selectedChoice == 1)
        {
            //user clicked sell so handle sell item logic
            //allow it to access inventory so user can pick an item from it's inventory to sell.
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);

        }
        else if (selectedChoice == 2)
        {
            //user selected quit so quit menu
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling, (selectedItem)=> StartCoroutine(SellItem(selectedItem)));
        }
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        //if user clicks back while selling go back to menu
        //also need to implement logic so when we click to sell and item we get prompted to sell not use the item. 
        StartCoroutine(StartMenuState());
        inventoryUI.gameObject.SetActive(false);
    }


    IEnumerator SellItem(ItemBase itemToSell)
    {
        state = ShopState.Busy;
        //first check if you can't sell item. if you cant print out sorry I cant accept that.
        if (!itemToSell.IsSellable)
        {
            yield return DIalogManager.Instance.ShowDialogText("Sorry I cant accept that Item.");
            state = ShopState.Selling;
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
            choices: new List<string>() { "Yes", "No"},
           onChoiceSelected: choiceIndex => selectedChoice= choiceIndex);

        if (selectedChoice == 0)
        {
            //yes they want to sell so logic to sell item.
            inventory.RemoveItem(itemToSell,countTosell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DIalogManager.Instance.ShowDialogText($"Turned over {itemToSell.Name}. You recieved ₽{sellingPrice}");
        }
        //closing wallet
        walletUI.Close(); 

        state = ShopState.Selling;//setting state back to selling so user can sell another item
    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;

        yield return DIalogManager.Instance.ShowDialogText("How many would you like to buy?",
            waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DIalogManager.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;

        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DIalogManager.Instance.ShowDialogText($"That will be ₽{totalPrice}",
                waitForInput: false,
                choices: new List<string>() { "Yes", "No" },
               onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                //user said yes
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DIalogManager.Instance.ShowDialogText("Thanks for shopping with us!");
            }

        }
        else
        {
            yield return DIalogManager.Instance.ShowDialogText("Not enough funds!");
        }

        state = ShopState.Buying;
    }

    IEnumerator OnBackFromBuying() {

        yield return GameController.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }

}
