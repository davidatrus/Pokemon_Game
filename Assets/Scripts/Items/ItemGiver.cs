using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour,ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count = 1;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DIalogManager.Instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;

        AudioManager.i.PlaySFX(AudioID.ItemObtained, pauseMusic: true);

        string dialogText = $"{player.Name} recieved {item.Name}";
        if(count >1 )
            dialogText = $"{player.Name} recieved {count} {item.Name}s";

        yield return DIalogManager.Instance.ShowDialogText(dialogText);
    }


    public bool CanBeGiven()
    {
        return item != null && count >0 && !used;
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
