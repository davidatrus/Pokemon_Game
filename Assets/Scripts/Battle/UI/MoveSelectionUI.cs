using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.SelectionUI;
using System.Linq;
using UnityEngine.UI;

public class MoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] List<TextSlot> moveTexts;
    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    List<Move> _moves;


    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
    }
    public void SetMoves(List<Move> moves)
    {
        _moves = moves;

        selectedItem = 0; //edge case when switching from pokemon w 4 moves to a pokemon with <4 moves  

        SetItems(moveTexts.Take(moves.Count).ToList());

        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].SetText(moves[i].Base.Name);
            else
                moveTexts[i].SetText("-");
        }
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

       var move = _moves[selectedItem];

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else
            ppText.color = Color.black;


    }
}
