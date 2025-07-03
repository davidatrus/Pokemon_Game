using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAction : CutSceneAction
{
    [SerializeField] float duration=0.5f;

    public override IEnumerator Play()
    {
        yield return Fader.i.FadeOut(duration);
    }
}
