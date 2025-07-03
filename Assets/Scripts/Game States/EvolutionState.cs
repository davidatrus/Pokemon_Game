using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PKMNGAMEUtils.StateMachine;
using static PokemonBase;

public class EvolutionState : State<GameController>
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;

    [SerializeField] AudioClip evolutionMusic;

    public static EvolutionState i { get; private set;}
    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        var gc = GameController.Instance;
        gc.StateMachine.Push(this);


        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evolutionMusic);


        pokemonImage.sprite = pokemon.Base.FrontSprite; // setting image of current pokemon evolving 
        yield return DIalogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is evolving!");
         
        var oldPokemon = pokemon.Base;

        pokemon.Evolve(evolution);

        pokemonImage.sprite = pokemon.Base.FrontSprite; // setting image to evolved pokemon

        yield return DIalogManager.Instance.ShowDialogText($"{oldPokemon.Name} has evolved into {pokemon.Base.Name}!");


        evolutionUI.SetActive(false);
        gc.PartyScreen.SetPartyData();
        AudioManager.i.PlayMusic(gc.CurrentScene.SceneMusic, fade: true);

        gc.StateMachine.Pop();

    }
}
