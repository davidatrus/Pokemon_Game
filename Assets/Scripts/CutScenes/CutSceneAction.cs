using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutSceneAction
{
    [SerializeField] string name;
    [SerializeField] bool shouldWeWait = true;

    //virtual func so we can implement in subclasses
    public virtual IEnumerator Play(){

        yield break;
    }



    public string Name
    {
        get => name;
        set => name = value;
    }

    public bool ShouldWeWait => shouldWeWait;
}
