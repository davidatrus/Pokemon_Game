using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new pokeball")]
public class PokeballItems : ItemBase
{
    [SerializeField] float catchRateModifer = 1;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }


    public override bool CanUseOustideBattle => false;


    public float CatchRateModifer => catchRateModifer;
}
