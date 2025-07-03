using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DIalogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox; 
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;


    public event Action OnShowDialog;
    public event Action OnDialogFinished;


    public static DIalogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

   

    public bool IsShowing { get; private set; }

    public IEnumerator ShowDialogText(string text, bool waitForInput =true, bool autoClose=true, List<string> choices = null,
        Action<int> onChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        AudioManager.i.PlaySFX(AudioID.UISelect);
       yield return TypeDialog(text);
        if (waitForInput)
        {
           yield return  new WaitUntil(() => Input.GetKeyDown(KeyCode.Z)); //waiting until player presses Z to close dialog.
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
       
    }


    public IEnumerator ShowDialog(Dialog dialog,List<string> choices=null,
        Action<int> onChoiceSelected=null) //setting null so its an option since not all dialog will have choices same with onChoiceSelected
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        foreach(var line in dialog.Lines)
        {
            AudioManager.i.PlaySFX(AudioID.UISelect);
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z)); 
        }

        if (choices!=null && choices.Count > 1)
        {
           yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    public void HandleUpdate()
    {
        
    }


    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

}
