using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HeightMeter : MonoBehaviour
{

    [SerializeField] private float value;
    [SerializeField] private float interval;
    [SerializeField] private Image imageLines;
    [SerializeField] private TextMeshProUGUI labelPrefab;
    private TextMeshProUGUI[] labels;

    private float tweenedValue;

    private void Start()
    {
        float height = ((RectTransform)transform).rect.height;
        float linesTextureHeight = imageLines.sprite.texture.height;
        int linesTextureRepeat = 1 + Mathf.FloorToInt(height / linesTextureHeight) | 1; // x / 2 * 2 + 1 == x | 1
        var linesSize = imageLines.rectTransform.sizeDelta;
        linesSize.y = linesTextureRepeat * linesTextureHeight;
        imageLines.rectTransform.sizeDelta = linesSize;
        
        labels = new TextMeshProUGUI[Mathf.CeilToInt(height / interval) + 1];
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = Instantiate(labelPrefab, transform);
        }
        UpdateLabels();
    }

    public void Add(float amount)
    {
        Set(value + amount);
    }

    public void Set(float value)
    {
        this.value = value;
        DOTween.To(() => tweenedValue, v =>
        {
            tweenedValue = v;
            var pos = imageLines.rectTransform.localPosition;
            pos.y = -(v % 50);
            imageLines.rectTransform.localPosition = pos;
            UpdateLabels();
        }, value, 0.5f).SetTarget(this).SetEase(Ease.OutCubic);
    }

    private void UpdateLabels()
    {
        float height = ((RectTransform)transform).rect.height;
        // int s = Mathf.RoundToInt(tweenedValue / interval);
        int startRow = Mathf.FloorToInt(tweenedValue / interval) - Mathf.FloorToInt(height / 2 / interval);
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].text = ((i + startRow) * interval).ToString();
            var pos = labels[i].rectTransform.anchoredPosition;
            pos.y = -tweenedValue + (i + startRow) * interval;
            labels[i].rectTransform.anchoredPosition = pos;
            // labels[i].text = ((s + i) * interval).ToString();
            // labels[i].rectTransform.anchoredPosition = new Vector2(0, height - i * interval + tweenedValue % interval);
        }
    }
    
}
