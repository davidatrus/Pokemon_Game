using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

public enum InventoryUIState { ItemSelection, PartySelection,MoveToForget,Busy}//using busy when items is being used.

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;


    Action<ItemBase> onItemUsed;

    int selectedItems = 0;
    int selectedCategory = 0;

    MoveBase moveToLearn;


    InventoryUIState state; 

    const int itemsInViewport = 8;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    RectTransform itemListRect;


    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();

    }

    private void Start()
    {
        UpdateItemsList();
        inventory.OnUpdated += UpdateItemsList;
    }

    void UpdateItemsList()
    {
        //clear all items on start, so remove all the children of the items list

        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        //attaching all the items in our player inventory to menu

        slotUIList = new List<ItemSlotUI>();
        foreach(var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
          var slotUIObj=Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }


    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;


        if(state== InventoryUIState.ItemSelection)
        {
            int previousSelection = selectedItems;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItems;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItems;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedCategory;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedCategory;

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;
            //lines 93- 96 allowing you to be able to rotate between selections not hard clamping you like i do when you go down/up items 

          selectedItems = Mathf.Clamp(selectedItems, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);


            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemsList();
            }
            else if (previousSelection != selectedItems)
            {
                UpdateItemSelection();
            }
            if (Input.GetKeyDown(KeyCode.Z))
                StartCoroutine(ItemSelected());

            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();
        }
        else if(state == InventoryUIState.PartySelection)
        {
            //party selection
            Action onSelected = () =>
            {
                //use selected item on selected pokemon
                StartCoroutine(UseItem());
               
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelected,onBackPartyScreen);
        }
        else if (state == InventoryUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
            
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItems, selectedCategory);

        if (GameController.Instance.State == GameState.Battle)
        {
            // in battle cant use TM/HM ITEMS.
            if (!item.CanUseInBattle)
            {
                yield return DIalogManager.Instance.ShowDialogText($"You can't use this item in battle!");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            //outside battle cant use Pokeballs
            if (!item.CanUseOustideBattle)
            {
                yield return DIalogManager.Instance.ShowDialogText($"You can only use this item in battle!");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }


        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is TMItem)
                //show if TM is able to be learnt by the pokemon.
                partyScreen.ShowIfTmIsUseable(item as TMItem);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

      var usedItem =  inventory.UseItem(selectedItems, partyScreen.SelectedMember,selectedCategory);
        if (usedItem != null)
        {
            if(usedItem is RecoveryItems)
          yield return  DIalogManager.Instance.ShowDialogText($"You Used a {usedItem.Name}");

            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory== (int)ItemCategory.Items)
                yield return DIalogManager.Instance.ShowDialogText($"It won't have any affect.");
        }

        ClosePartyScreen();

    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItems, selectedCategory) as TMItem;
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
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);

        }
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DIalogManager.Instance.ShowDialogText($"Choose a move you want to forget.",true,false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItems = Mathf.Clamp(selectedItems, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItems)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if (slots.Count > 0)
        {
            var item = slots[selectedItems].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }


        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;

        float  scrollPos = Mathf.Clamp(selectedItems - itemsInViewport/2, 0, selectedItems) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x,scrollPos);

        bool showUpArrow = selectedItems > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItems + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
        // items in viewport i know max amount i can base see is 8 on screen, so once i go past my 5th item thats when i want to start scrolling, and after if started scrolling show the up and down arrows acordingly.
    }

    void ResetSelection()
    {
        selectedItems = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
            partyScreen.gameObject.SetActive(true);
         
    }
    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        DIalogManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            //dont learn new move
            yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}.");
        }
        else
        {
            //learn the new move by forgeting selected move.
            var selectedMove = pokemon.Moves[moveIndex].Base;
            yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}.");

            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}
