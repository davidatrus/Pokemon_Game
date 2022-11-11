using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    List<ItemBase> availableItems;

    Action<ItemBase> onItemSelected;
    Action onBack;

    List<ItemSlotUI> slotUIList;

    const int itemsInViewport = 8;

    int selectedItems;

    RectTransform itemListRect;


    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void ShowUI(List<ItemBase> availableItems,Action<ItemBase> onItemSelected,
        Action onBack)
    {
        this.availableItems = availableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;


        gameObject.SetActive(true);
        UpdateItemsList();

    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
    {
        var prevSelection = selectedItems;

        //function handling what to do on user input. 
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItems;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItems;

        selectedItems = Mathf.Clamp(selectedItems, 0, availableItems.Count - 1);

        //handling switching of item when user goes up or down through shop
        if (selectedItems != prevSelection)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
            onItemSelected?.Invoke(availableItems[selectedItems]);
        else if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
    }

    void UpdateItemsList()
    {
        //clear all items on start, so remove all the children of the items list

        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        //attaching all the items in our player inventory to menu

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in availableItems)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetNameAndPrice(item);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }
    void UpdateItemSelection()
    {
        selectedItems = Mathf.Clamp(selectedItems, 0, availableItems.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItems)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if (availableItems.Count > 0)
        {
            var item = availableItems[selectedItems];
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }


        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItems - itemsInViewport / 2, 0, selectedItems) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItems > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItems + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
        // items in viewport i know max amount i can base see is 8 on screen, so once i go past my 5th item thats when i want to start scrolling, and after if started scrolling show the up and down arrows acordingly.
    }
}
