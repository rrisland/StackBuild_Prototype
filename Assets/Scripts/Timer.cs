using System.Collections;
using System.Collections.Generic;
using StackProto;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float time;
    [SerializeField] private FinishSequence finishSequence;

    private void Update()
    {
        if (time <= 0) return;

        time -= Time.deltaTime;

        if (time <= 0 && finishSequence)
        {
            time = 0;
            finishSequence.DisplayAsync().Forget();
        }

        text.text = "Time: " + time.ToString("000.00");
    }
}
