using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TransitionUIController : MonoBehaviour
{
    public static TransitionUIController Instance;
    [SerializeField] Image fadingImage;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ForceFadeOut()
    {
        fadingImage.raycastTarget = true;
        fadingImage.color = Color.black;
    }

    public void FadeOut()
    {
        fadingImage.raycastTarget = true;
        fadingImage.DOColor(Color.black, 1.2f);
    }

    public void FadeIn()
    {
        fadingImage.DOColor(Color.clear, 1.2f);
        fadingImage.raycastTarget = false;
    }
}
