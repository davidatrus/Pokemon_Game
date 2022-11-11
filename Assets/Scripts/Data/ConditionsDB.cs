using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

   

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name= "Poison",
                StartMessage= "has been poisoned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                pokemon.DecreaseHP(pokemon.MaxHP/8);
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is hurt by poison!");
              }
           }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name= "Burn",
                StartMessage= "has been burned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                pokemon.DecreaseHP((pokemon.MaxHP/16)+1);
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is hurt by its burn!");
              }
           }
        },
         {
            ConditionID.par,
            new Condition()
            {
                Name= "Paralyzed",
                StartMessage= "has been paralyzed",
                OnStart= (Pokemon pokemon)=>
                {
                   pokemon.StatusTime=Random.Range(1,5);
                   Debug.Log($"Will be paralyzed for {pokemon.StatusTime} moves");
                },
                OnBeforeMove= (Pokemon pokemon) =>
                {
                     if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is no longer paralyzed!");
                        return true;// so if the  status time is less than or equal to 0 then pokemon is out of the volatile condition, call in cure status, 
                    }
                    //else if its statustime still greater than 0, decrement it by 1, give it a 25% chance to perform a move if it cant dont perform move.

                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed!");
                    if(Random.Range(1,5)==1)
                        return true;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed! It cant move!");
                    return false;
                }

           }
        },
         {
            ConditionID.frz,
            new Condition()
            {
                Name= "Frozen",
                StartMessage= "has been frozen",
                OnStart= (Pokemon pokemon)=>
                {
                 //sleep for 1-3 turns
                    pokemon.StatusTime= Random.Range(1,4);
                    Debug.Log($"Will be frozen for {pokemon.StatusTime} moves");
                },
                OnBeforeMove= (Pokemon pokemon)=>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} Thawed Out!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is frozen solid!");
                    return false;
                }
           }
        },
         {
            ConditionID.slp,
            new Condition()
            {
                Name= "Sleep",
                StartMessage= "has fallen asleep",
                OnStart= (Pokemon pokemon) =>
                {
                    //frozen for 1-3 turns
                    pokemon.StatusTime= Random.Range(1,4);
                    Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                },
                OnBeforeMove= (Pokemon pokemon)=>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} Woke up!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping...");
                    return false;
                }
           }
        },
        /*{
            ConditionID.speed,
            new Condition()
            {
                Name= "Speed",
                StartMessage= "speed has fallen.",
                OnStart= (Pokemon pokemon) =>
                {
                    //frozen for 1-3 turns
                    pokemon.StatusTime= Random.Range(1,4);
                    Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                },
                OnBeforeMove= (Pokemon pokemon)=>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} Woke up!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping...");
                    return false;
                }
           }
        }, */
         {//confusion is a volatile status. 
            ConditionID.confusion,
            new Condition()
            {
                Name= "Confusion",
                StartMessage= "has been confused",
                OnStart= (Pokemon pokemon) =>
                {
                    //confused  for 1-4 turns
                    pokemon.VolatileStatusTime= Random.Range(1,5);
                    Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} moves");//for my use just to make sure code works properly.
                },
                OnBeforeMove= (Pokemon pokemon)=>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} snapped out of confusion!");
                        return true;// so if the volatile status time is less than or equal to 0 then pokemon is out of the volatile condition, call in cure volatile, display text of snapping out.
                    }
                    //else if its volatilestatustime still greater than 0, decrement it by 1, give it a 50% chance to perform a move if it cant hurt pokemon by confusion
                    pokemon.VolatileStatusTime--;

                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused!");
                    if(Random.Range(1,3)==1)
                        return true;
                    pokemon.DecreaseHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue("It Hurt itself due to confusion!");
                    return false;
                        
                }
           }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn||condition.Id==ConditionID.brn)
            return 1.5f;

        return 1f;
    }

}


public enum ConditionID
{
    none,psn,brn,slp,par,frz,confusion,speed
}
