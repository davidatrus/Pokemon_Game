using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace PKMNGAMEUtils.SelectionUI
{
    public enum SelectionType {List,Grid}

    //making sure that T is always from my IselectableItem interface
    public class SelectionUI<T> : MonoBehaviour where T: ISelectableItem 
    {
        List<T> items;
        protected int selectedItem = 0;

        SelectionType selectionType;
        int gridWidth = 2; //party screen is a grid since 6 pokemon total width is 2 

        public event Action<int> OnSelected; //user pressing z  event so no other states can override these actions
        public event Action OnBack; //user pressing x


        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            this.selectionType = selectionType;
            this.gridWidth = gridWidth;
        }

        public void SetItems(List<T> items)
        {
            this.items = items;
            //gaurentes that all the text items will show up before anything else not only when handle selction happens
            items.ForEach(i => i.Init());
            UpdateSelectionInUI();
        }

        public void ClearItems()
        {
            items.ForEach(i => i.Clear());

            this.items = null;
        }

        public virtual void HandleUpdate()
        {
            int prevSelection = selectedItem;

            if (selectionType == SelectionType.List)
                HandleListSelection();
            else if (selectionType == SelectionType.Grid)
                HandleGridSelection();

            
            selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);

            
            if (selectedItem != prevSelection)
                UpdateSelectionInUI();

            if (Input.GetKeyDown(KeyCode.Z))
                OnSelected?.Invoke(selectedItem);
            else if (Input.GetKeyDown(KeyCode.X))
                OnBack?.Invoke();
                

        }

        void HandleListSelection()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
        }

        void HandleGridSelection()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedItem;
        }

        public virtual void UpdateSelectionInUI()
        {
            for (int i = 0; i< items.Count; i++)
            {
                items[i].OnSelectionChanged(i == selectedItem);
            }
        }

    }
}