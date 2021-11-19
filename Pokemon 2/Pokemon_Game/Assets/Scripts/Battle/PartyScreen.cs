using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PartyScreen : MonoBehaviour
{
   [SerializeField] Text messageText;

    PartyMemberUI[]  memberSlot;
    List<Pokemon> pokemons;
    PokemonParty party;

    int selection = 0;

    public Pokemon SelectedMember => pokemons[selection];


    /// <summary>
    /// allowing party screen to be called from multiple places (Running Turn,AboutToUse,ActionSelection)
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        memberSlot = GetComponentsInChildren<PartyMemberUI>(true);
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

        UpdateMemberSelection(selection);
        
        messageText.text = "Choose a Pokemon";
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var previousSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, pokemons.Count - 1);


        if(selection!=previousSelection)
            UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for(int i = 0; i < pokemons.Count; i++)
        {
            if (i == selectedMember)
                memberSlot[i].SetSelected(true);
            else
                memberSlot[i].SetSelected(false);
        }
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
