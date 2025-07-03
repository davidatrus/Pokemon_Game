using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class SurfWater : MonoBehaviour, Interactable, IPlayerTriggable
{
    bool isJumpingIntoWater = false;

    //trigger while moving through water
    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharacterAnimator>();
        //checking if we are jumping into water or already surfing, if we are dont run rest of code. 
        if (animator.IsSurfing || isJumpingIntoWater)
            yield break;

        yield return DIalogManager.Instance.ShowDialogText("The water is deep blue...");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Surf"));
        if (pokemonWithSurf != null)
        {
            int selectedChoice = 0;
            yield return DIalogManager.Instance.ShowDialogText($"Should {pokemonWithSurf.Base.Name} use Surf?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                //Yes
                yield return DIalogManager.Instance.ShowDialogText($"{pokemonWithSurf.Base.Name} used Surf");

                //move player one tile in the direction they are facing, grabing from character animator script

               
               var direction = new Vector3(animator.MoveX, animator.MoveY);
               var targetPos = initiator.position + direction;

                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                isJumpingIntoWater = true;
                animator.IsSurfing = true;


            }

        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) <= 10)
        {
            GameController.Instance.StartBattle(BattleTrigger.Water);
        }
    }

}
