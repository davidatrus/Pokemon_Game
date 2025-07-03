using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public event Action OnUpdated;

    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
        set
        {
            pokemons = value;
            OnUpdated?.Invoke();

        }

    }

    private void Awake()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    private void Start()
    {
        
    }

    public Pokemon GetHealthyPokemon()
    {
       return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
            OnUpdated?.Invoke();
        }
        else
        {
            //TODO implement PC to send pokemon to PC. 
        }
    }

    public bool CheckForEvoultions()
    {
        return pokemons.Any(p => p.CheckForEvolution() != null);
    }

    public IEnumerator RunEvolutions()
    {
        foreach(var pokemon in pokemons)
        {
           var evolution = pokemon.CheckForEvolution();
            if (evolution != null)
            {
               yield return EvolutionState.i.Evolve(pokemon, evolution);
            }
        }
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
        //allows us to call the onupdated event from outside this class. so can call from healer script
    }

    public static PokemonParty GetPlayerParty()
    {
       return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }

   /* public void HealParty()
    {
        foreach (Pokemon mon in pokemons)
        {
            mon.CureStatus();
            mon.CureVolatileStatus();
            mon.RestoreHP();
            mon.ResetStatBoost();

            foreach (Move mov in mon.Moves)
            {
                mov.PP = mov.Base.PP;
            }
        }
        Debug.Log("You've Healed your pokemon");
    } */
}
