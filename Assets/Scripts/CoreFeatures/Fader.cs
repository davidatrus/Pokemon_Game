using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fader : MonoBehaviour
{
    public static Fader i { get; private set; } //making fader class singleton to be able to access instance elsewhere

    Image image;

    private void Awake()
    {
        i = this;// assigning instance
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float time)
    {
       yield return  image.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time)
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }
}
