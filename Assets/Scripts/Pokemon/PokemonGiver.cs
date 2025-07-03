using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] Pokemon pokemonToGive;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GivePokemon(PlayerController player)
    {
        yield return DIalogManager.Instance.ShowDialog(dialog);

        pokemonToGive.Init();
        player.GetComponent<PokemonParty>().AddPokemon(pokemonToGive); 

        used = true;

        AudioManager.i.PlaySFX(AudioID.PokemonObtained, pauseMusic: true);

        string dialogText = $"{player.Name} recieved {pokemonToGive.Base.Name}";

        yield return DIalogManager.Instance.ShowDialogText(dialogText);
    }


    public bool CanBeGiven()
    {
        return pokemonToGive != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
