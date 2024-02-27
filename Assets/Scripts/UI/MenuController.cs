using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PKMNGAMEUtils.SelectionUI;

public class MenuController : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }

    //[SerializeField] GameObject menu;

    //public event Action<int> onMenuSelected;
    //public event Action onBack;

    //List<Text> menuItems;

    //int selectedItems = 0;

    //private void Awake()
    //{
    //   menuItems= menu.GetComponentsInChildren<Text>().ToList();
    //}

    //public void OpenMenu()
    //{
    //    menu.SetActive(true);
    //    UpdateItemSelection();
    //}

    //public void CloseMenu()
    //{
    //    menu.SetActive(false);
    //}

    //public void HandleUpdate()
    //{
    //    int previousSelection = selectedItems;

    //    if (Input.GetKeyDown(KeyCode.DownArrow))
    //        ++selectedItems;
    //    else if (Input.GetKeyDown(KeyCode.UpArrow))
    //        --selectedItems;

    //    selectedItems = Mathf.Clamp(selectedItems, 0, menuItems.Count - 1);

    //    if (previousSelection != selectedItems)
    //        UpdateItemSelection();

    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        onMenuSelected?.Invoke(selectedItems);
    //        CloseMenu();
    //    }
    //    else if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        onBack?.Invoke();
    //        CloseMenu();
    //    }

    //}

    //void UpdateItemSelection()
    //{
    //    for(int i = 0; i <menuItems.Count; i++)
    //    {
    //        if (i == selectedItems)
    //            menuItems[i].color = GlobalSettings.i.HighlightedColor;
    //        else
    //            menuItems[i].color = Color.black;
    //    }
    //}
}
