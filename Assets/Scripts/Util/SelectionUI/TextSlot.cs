using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// file for handling selection for texts, change text color depending on what is selected.
/// </summary>
public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Text text;

    Color orginalColor;

    public void Init()
    {
        orginalColor = text.color;
    }
    public void OnSelectionChanged(bool selected)
    {
        text.color = (selected) ? GlobalSettings.i.HighlightedColor : orginalColor;
    }

    public void SetText(string s)
    {
        text.text = s;
    }

    public void Clear()
    {
        text.color = orginalColor;
    }
}
