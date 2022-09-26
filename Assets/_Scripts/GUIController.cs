using System.Collections;
using System.Collections.Generic;
using game;
using TMPro;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    [field: SerializeField] public TMP_Text waterText;
    [field: SerializeField] public Animation waterChangedAnimation;

    public void OnWaterChanged(GameManager.OnWaterChangedEventArgs args)
    {
        waterText.text = $"{args.currentAmount}";
        waterChangedAnimation.Play();
    }
}
