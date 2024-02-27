using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class ShopBuyingState : State<GameController>
{
    [SerializeField] ShopUI shopUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] Vector2 shopCameraOffset;

    public static ShopBuyingState i { get; private set; }

    bool browseItems = false;
    private void Awake()
    {
        i = this;
    }
    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory(); // getting instance of inventory 
    }

    public List<ItemBase> AvailableItems { get; set; }
    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;
        browseItems = false;
        StartCoroutine(StartBuyingState());

    }

    public override void Execute()
    {
        if(browseItems)
        shopUI.HandleUpdate();
    }

    IEnumerator StartBuyingState()
    {
        yield return GameController.Instance.MoveCamera(shopCameraOffset);
        walletUI.Show();
        shopUI.ShowUI(AvailableItems, (item) => StartCoroutine(BuyItem(item)),
           () => StartCoroutine(OnBackFromBuying()));
        browseItems = true;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        browseItems = false;
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
        browseItems = true;
    }

    IEnumerator OnBackFromBuying()
    {

        yield return GameController.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        gc.StateMachine.Pop();
    }
}
