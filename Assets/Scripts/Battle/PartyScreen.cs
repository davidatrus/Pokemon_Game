using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PKMNGAMEUtils.SelectionUI;
using System.Linq;


public class PartyScreen : SelectionUI<TextSlot>
{
   [SerializeField] Text messageText;

    PartyMemberUI[]  memberSlot;
    List<Pokemon> pokemons;
    PokemonParty party;

   

    public Pokemon SelectedMember => pokemons[selectedItem];


  

    public void Init()
    {
        memberSlot = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.Grid, 2);

        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        pokemons = party.Pokemons;

        for(int i =0; i < memberSlot.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlot[i].gameObject.SetActive(true);
                memberSlot[i].Init(pokemons[i]);
            }
            else
                memberSlot[i].gameObject.SetActive(false);
        }

       var textSlots = memberSlot.Select(m => m.GetComponent<TextSlot>());
        SetItems(textSlots.Take(pokemons.Count).ToList());

        
        messageText.text = "Choose a Pokemon";


    }



    public void ShowIfTmIsUseable(TMItem tmItem)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            string message = tmItem.CanBeTaught(pokemons[i]) ? "ABLE!" : "NOT ABLE!";
            memberSlot[i].SetMessage(message);
        }
    }

    public void ClearMemberSlotMessages()
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            memberSlot[i].SetMessage("");
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
