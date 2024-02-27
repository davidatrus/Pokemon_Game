using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryScreenUI : MonoBehaviour
{
    [Header("Basic Details")]
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Image image;


    [Header("Pages")]
    [SerializeField] Text pageNameText;
    [SerializeField] GameObject skillsPage;
    [SerializeField] GameObject movesPage;

    [Header("Pokemon Skills")]
    [SerializeField] Text hpText;
    [SerializeField] Text attackText;
    [SerializeField] Text defenseText;
    [SerializeField] Text spAttackText;
    [SerializeField] Text spDefText;
    [SerializeField] Text speedText;
    [SerializeField] Text expText;
    [SerializeField] Text nextLevelText;
    [SerializeField] Transform expBar;

    [Header("PokemonMoves")]
    [SerializeField] List<Text> moveTypes;
    [SerializeField] List<Text> moveNames;
    [SerializeField] List<Text> movePP;


    Pokemon pokemon;

    public void SetBasicDetails(Pokemon pokemon)
    {
        this.pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv. " + pokemon.Level;
        image.sprite = pokemon.Base.FrontSprite;
    }

    public void ShowPage(int pageNumber)
    {
        if (pageNumber == 0)
        {
            //skils page
            pageNameText.text = "Pokemon Skills";
            skillsPage.SetActive(true);
            movesPage.SetActive(false);
            SetSkills();
        }
        else if (pageNumber == 1)
        {
            //moves page
            pageNameText.text = "Pokemon Moves";
            skillsPage.SetActive(false);
            movesPage.SetActive(true);
            SetMoves();
        }
    }

    public void SetSkills()
    {
        hpText.text = $"{pokemon.HP}/{pokemon.MaxHP}";
        attackText.text = "" + pokemon.Attack;
        defenseText.text = "" + pokemon.Defence;
        spAttackText.text = "" + pokemon.SpAttack;
        spDefText.text = "" + pokemon.SpDefence;
        speedText.text = "" + pokemon.Speed;

        expText.text = "" + pokemon.Exp;
        nextLevelText.text = "" + (pokemon.Base.GetExpForLevel(pokemon.Level + 1) - pokemon.Exp);
        expBar.localScale = new Vector2(pokemon.GetNormalizedExp(), 1);
    }

    public void SetMoves()
    {
        for (int i = 0; i < moveNames.Count; i++)
        {
            if (i< pokemon.Moves.Count)
            {
               var move = pokemon.Moves[i];
                moveTypes[i].text = move.Base.Type.ToString().ToUpper();
                moveNames[i].text = move.Base.Name.ToUpper();
                movePP[i].text = $"PP {move.PP}/{move.Base.PP}";

            }
            else
            {
                //no moves so leave as blank
                moveTypes[i].text = "-";
                moveNames[i].text = "-";
                movePP[i].text = "-";
            }
        }
    }

} 
