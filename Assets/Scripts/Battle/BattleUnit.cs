using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//-198
public class BattleUnit : MonoBehaviour
{
    
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public  BattleHud Hud
    {
        get { return hud; }
    }
    

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public Pokemon Pokemon { get; set; }

    Image image;
    Vector3 orginalPos;
    Color originalColor;    

    private void Awake()
    {
        image = GetComponent<Image>();
        orginalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Pokemon pokemon)
    {
       Pokemon =  pokemon;
        if (isPlayerUnit)   
           image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);

        transform.localScale = new Vector3(0.7f, 0.7f, 1);
        image.color = originalColor;

        PlayEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, orginalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, orginalPos.y);

        image.transform.DOLocalMoveX(orginalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray,0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orginalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

   public IEnumerator PlayCaptureAnimation()
    {
        var sequences = DOTween.Sequence();
        sequences.Append(image.DOFade(0, 0.5f));
        sequences.Join(transform.DOLocalMoveY(orginalPos.y + 50f, 0.5f));
        sequences.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequences.WaitForCompletion();
    }
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequences = DOTween.Sequence();
        sequences.Append(image.DOFade(1, 0.5f));
        sequences.Join(transform.DOLocalMoveY(orginalPos.y , 0.5f));
        sequences.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequences.WaitForCompletion();
    }
}
