using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HeightMeterTestButton : MonoBehaviour
{

    private enum Type
    {
        Add,
        Set,
    }
    
    [SerializeField] private Button button;
    [SerializeField] private HeightMeter meter;
    [SerializeField] private Type type;
    [SerializeField] private float value;

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            switch (type)
            {
                case Type.Add: meter.Add(value); break;
                case Type.Set: meter.Set(value); break;
                default: throw new ArgumentOutOfRangeException($"Unknown meter type {type}");
            }
        });
    }
}
