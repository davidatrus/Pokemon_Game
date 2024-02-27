using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] List<PokemonEncounterRecord> waterPokemons;

    [HideInInspector]
    [SerializeField] int totalChance = 0;
    [HideInInspector]
    [SerializeField] int totalChanceWater = 0;

    private void OnValidate()
    {
        CalculateChancePercentage();
    }

    private void Start()
    {
        CalculateChancePercentage();
    }

    void CalculateChancePercentage()
    {
        totalChance = -1;
        totalChanceWater = -1;

        if (wildPokemons.Count > 0)
        {
            totalChance = 0;
            foreach (var record in wildPokemons)
            {
                record.chanceLower = totalChance;
                record.chanceHigher = totalChance + record.chancePercentage;

                totalChance = totalChance + record.chancePercentage;
                //doing chance percentage like a list, so if we have 3 pokemon x y z, x&y have a 40% chance and z has 20%,
                // pokemon x will spawn from 0-40, y from 41-80 and z from 81-100.
                //make it so if random value generated is 27 give us pokemon x, etc etc 
            }
        }

        if (waterPokemons.Count > 0)
        {
            totalChanceWater = 0;
            foreach (var record in waterPokemons)
            {
                record.chanceLower = totalChanceWater;
                record.chanceHigher = totalChanceWater + record.chancePercentage;

                totalChanceWater = totalChanceWater + record.chancePercentage;
            }
        }
    }

    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        //using certain pokemon list depending on if in grass or water
        var pokemonList =  (trigger == BattleTrigger.LongGrass) ? wildPokemons : waterPokemons;

        int randomValue = Random.Range(1, 101);
        var pokemonRecord = pokemonList.First(p => randomValue >= p.chanceLower && randomValue <= p.chanceHigher);

        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y+1);

        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        wildPokemon.Init();
        return wildPokemon;
    }
}

[System.Serializable]

public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceHigher { get; set; }

}