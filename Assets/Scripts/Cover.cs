using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float appearanceTime = 15.0f;
    [SerializeField] private float intervalTime = 5.0f;

    private void Awake()
    {
        if (meshRenderer is null) TryGetComponent(out meshRenderer);
        if (meshCollider is null) TryGetComponent(out meshCollider);
    }
    
    private void Start()
    {
        if (meshRenderer is null || meshCollider is null) return;
        
        StartCoroutine(AppearanceTimer());
    }
    
    private IEnumerator AppearanceTimer()
    {
        while (true)
        {
            meshRenderer.enabled = true;
            meshCollider.enabled = true;
            yield return new WaitForSeconds(appearanceTime);
            meshRenderer.enabled = false;
            meshCollider.enabled = false;
            yield return new WaitForSeconds(intervalTime);
        }
    }
}
