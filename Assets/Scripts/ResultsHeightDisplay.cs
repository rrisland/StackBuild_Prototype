using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ResultsHeightDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text heightText;
    [SerializeField] private TMP_Text winText;

    public void SetHeight(float height)
    {
        heightText.text = $"{height:F1}m";
    }

    public async UniTask DisplayWinAsync()
    {
        await DOTween.Sequence()
            .Join(winText.DOFade(1, 0.35f).SetEase(Ease.InCubic))
            .Join(winText.transform.DOScale(1, 0.35f).From(3).SetEase(Ease.InCubic));
    }

}
