using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PokemonBase;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;

    public static EvolutionManager i { get; private set;}
    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);

        pokemonImage.sprite = pokemon.Base.FrontSprite; // setting image of current pokemon evolving 
        yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is evolving!");

        var oldPokemon = pokemon.Base;

        pokemon.Evolve(evolution);

        pokemonImage.sprite = pokemon.Base.FrontSprite; // setting image to evolved pokemon

        yield return DIalogManager.Instance.ShowDialogText($"{oldPokemon.Name} has evolved into {pokemon.Base.Name}!");

        evolutionUI.SetActive(false);
        OnCompleteEvolution?.Invoke();

    }
}
