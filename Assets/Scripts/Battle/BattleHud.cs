using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;


    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;
    [SerializeField] Color slpColor;

    Pokemon _pokemon;

    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon)
    {
        if(_pokemon != null)
        {
            _pokemon.OnHPChanged -= UpdateHP;
            _pokemon.OnStatusChanged -= SetStatusText;

        }


        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHP);
        SetExp();

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.par, parColor},
            {ConditionID.frz, frzColor},
            {ConditionID.slp, slpColor},
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnHPChanged += UpdateHP;
    }

    void SetStatusText()
    {
       if (_pokemon.Status == null)
        {
            statusText.text = "";//if doesnt have a status hcange the text to blank.
        }
       else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public void SetLevel()
    {
        levelText.text = "Lv " + _pokemon.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;

       float normalizedExp= _pokemon.GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }
    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if(reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = _pokemon.GetNormalizedExp();
       yield return  expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

 

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHP);      
    } 


    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (_pokemon != null)
        {
            _pokemon.OnHPChanged -= UpdateHP;
            _pokemon.OnStatusChanged -= SetStatusText;

        }

    }
}