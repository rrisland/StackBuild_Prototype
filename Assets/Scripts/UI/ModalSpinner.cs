using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ModalBase))]
public class ModalSpinner : MonoBehaviour
{

    [SerializeField] private ModalBase modal;
    [SerializeField] private Image[] topImages;
    [SerializeField] private Image[] bottomImages;

    private void Reset()
    {
        modal = GetComponent<ModalBase>();
    }

    private void Awake()
    {
        modal.OnOpen.Subscribe(_ =>
        {
            for (int i = 0; i < topImages.Length; i++)
            {
                AnimateSpinner(topImages[i], true)
                    .SetDelay(i * 0.1f, false)
                    .SetTarget(this);
            }
            for (int i = 0; i < bottomImages.Length; i++)
            {
                AnimateSpinner(bottomImages[bottomImages.Length - 1 - i], false)
                    .SetDelay(i * 0.1f, false)
                    .SetTarget(this);
            }
        });
        modal.OnClose.Subscribe(_ =>
        {
            DOTween.Kill(this);
        });
    }

    private Sequence AnimateSpinner(Image img, bool clockwise)
    {
        return DOTween.Sequence()
            .Append(img.rectTransform.DOScaleX(-1, 0.3f).From(1))
            .AppendInterval(0.7f)
            .AppendCallback(() => img.fillClockwise = !clockwise)
            .Append(img.DOFillAmount(0, 0.15f).From(1))
            .AppendCallback(() => img.fillClockwise = clockwise)
            .Append(img.DOFillAmount(1, 0.15f).From(0, false))
            .AppendInterval(0.7f)
            .SetLoops(-1);
    }
    
}
