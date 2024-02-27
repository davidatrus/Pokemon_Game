using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.SelectionUI;
using System.Linq;

public class ActionSectionUI : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
