using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName ="Items/Create new recovery item")]
public class RecoveryItems : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;//potions, super potions hyper potions
    [SerializeField] bool restoreMaxHP;//max potion

    [Header("PP")]
    [SerializeField] int ppAmount;//ether
    [SerializeField] bool restoreMaxPP;//max ether

    [Header("Status Conditions")]
    [SerializeField] ConditionID status;//antidoate, burn heal,awkaning, paralyze heal 
    [SerializeField] bool recoverAllStatus;//full heal 

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;



    public override bool Use(Pokemon pokemon)
    {
        //dealing with revive / max revive if HP is greater than 0 ie alive dont run anything if fainted and using revive heal half of its max hp
        if(revive || maxRevive)
        {
            if (pokemon.HP > 0)
                return false;

            if (revive)
                pokemon.IncreaseHP(pokemon.MaxHP / 2);

            else if (maxRevive)
                pokemon.IncreaseHP(pokemon.MaxHP);

            pokemon.CureStatus();

            return true;
        }

        if (pokemon.HP == 0)
            return false;//dont allow fainted pokemon to use healing items

        //restoring HP 
       if(restoreMaxHP ||  hpAmount> 0)
        {
            if (pokemon.HP == pokemon.MaxHP)
                return false;

            if (restoreMaxHP)
                pokemon.IncreaseHP(pokemon.MaxHP);
            else
                pokemon.IncreaseHP(hpAmount);
        }

       if(recoverAllStatus|| status != ConditionID.none)
        {
            if (pokemon.Status == null && pokemon.VolatileStatus == null)
                return false;

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.Id == status)
                    pokemon.CureStatus();
                else if (pokemon.VolatileStatus.Id == status)
                    pokemon.CureVolatileStatus();
                else
                    return false;
            }
        }
        //restoring PP
        if (restoreMaxPP)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        }
        else if (ppAmount > 0)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }

        return true;
    }

}
