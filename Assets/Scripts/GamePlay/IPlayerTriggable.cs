using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTriggable 
{
    void OnPlayerTriggered(PlayerController player);

    bool TriggerRepeatedly { get;}
}
