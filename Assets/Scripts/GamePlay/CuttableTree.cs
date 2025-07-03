using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CuttableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DIalogManager.Instance.ShowDialogText("This tree looks like it can be cut down!");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Cut"));
        if (pokemonWithCut != null)
        {
            int selectedChoice = 0;
            yield return DIalogManager.Instance.ShowDialogText($"Should {pokemonWithCut.Base.Name} use cut?",
                choices: new List<string>() {"Yes", "No"},
                onChoiceSelected: (selection) => selectedChoice = selection);

            if(selectedChoice==0)
            {
                //Yes
                yield return DIalogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} used Cut");
                gameObject.SetActive(false);
            }

        }
    }
}
