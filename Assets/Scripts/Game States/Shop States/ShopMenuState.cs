using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;

public class ShopMenuState : State<GameController>
{
    public static ShopMenuState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    //input
    public List<ItemBase> AvailableItems { get; set; }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        StartCoroutine(StartMenuState());
    }

    IEnumerator StartMenuState()
    {

        int selectedChoice = 0;
        yield return DIalogManager.Instance.ShowDialogText("Welcome! What do you need?",
            waitForInput: false,
            choices: new List<string>() { "Buy", "Sell", "Quit" },
           onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //user clicked buy so buying items logic
            ShopBuyingState.i.AvailableItems = AvailableItems;
           yield return gc.StateMachine.PushAndWait(ShopBuyingState.i);
        }
        else if (selectedChoice == 1)
        {
            //user clicked sell so handle sell item logic
            //allow it to access inventory so user can pick an item from it's inventory to sell
           yield return gc.StateMachine.PushAndWait(ShopSellingState.i);

        }
        else if (selectedChoice == 2)
        {
           //quit
        }
        gc.StateMachine.Pop();
    }
}
