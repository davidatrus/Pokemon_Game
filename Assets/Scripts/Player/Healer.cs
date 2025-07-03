using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    //script to allow npc to heal our pokemon

    //takes player transform and take dialog that we define in npc controller
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;

       yield return DIalogManager.Instance.ShowDialogText("You look tired would you like to rest?",
           choices: new List<string>() { "Yes" , "No" },
           onChoiceSelected: (choiceIndex)=> selectedChoice= choiceIndex );
        //after showing dialog heal pokemon part by getting compentent of player party

        if (selectedChoice == 0)
        {
            //player selected yes so heal
            yield return Fader.i.FadeIn(0.55f); // fade in 

            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.55f); //fade out
            yield return DIalogManager.Instance.ShowDialogText($"Your Pokemon should be fully healed now! Be safe out there.");
        }
        else if (selectedChoice == 1)
        {
            //player selected no so dont heal.
            yield return DIalogManager.Instance.ShowDialogText($"You can always come back if you change your mind.");
        }

    }
}
