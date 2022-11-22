using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ModalBase : MonoBehaviour
{

    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform lineTop;
    [SerializeField] private RectTransform lineBottom;
    [SerializeField] private RectTransform background;
    [SerializeField] private CanvasGroup content;

    private bool isOpen;
    private readonly Subject<Unit> onOpen = new();
    private readonly Subject<Unit> onClose = new();

    public bool IsOpen => isOpen;
    public IObservable<Unit> OnOpen => onOpen;
    public IObservable<Unit> OnClose => onClose;

    private void Reset()
    {
        group = GetComponent<CanvasGroup>();
    }

    public async UniTaskVoid ShowAsync()
    {
        isOpen = true;
        gameObject.SetActive(true);
        onOpen.OnNext(Unit.Default);
        await DOTween.Sequence()
            .Join(group.DOFade(1, 0.1f).From(0))
            .Join(lineTop.DOScaleX(1, 0.3f).From(0).SetEase(Ease.OutCubic))
            .Join(lineBottom.DOScaleX(1, 0.3f).From(0).SetEase(Ease.OutCubic))
            .Join(background.DOScaleX(1, 0.3f).From(0).SetDelay(0.15f).SetEase(Ease.OutCubic))
            .Join(content.DOFade(1, 0.2f).From(0).SetDelay(0.05f));
    }

    public async UniTaskVoid HideAsync()
    {
        onClose.OnNext(Unit.Default);
        await DOTween.Sequence()
            .Join(content.DOFade(0, 0.1f).From(1))
            .Append(group.DOFade(0, 0.3f).From(1))
            .Join(lineTop.DOScaleX(0, 0.3f).From(1))
            .Join(lineBottom.DOScaleX(0, 0.3f).From(1))
            .Join(background.DOScaleX(0, 0.3f).From(1));
        gameObject.SetActive(false);
        isOpen = false;
    }
    
}
