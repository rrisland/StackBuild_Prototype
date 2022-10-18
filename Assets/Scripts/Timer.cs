using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float time;

    private void Update()
    {
        time -= Time.deltaTime;

        text.text = "Time: " + time.ToString("000.00");
    }
}
