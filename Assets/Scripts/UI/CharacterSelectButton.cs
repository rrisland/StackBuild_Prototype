using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterSelectButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    [SerializeField] private Button button;
    [SerializeField] private RectTransform[] cornerIndicators;

    public IObservable<Unit> OnClick => button.OnClickAsObservable();

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
    }

}
