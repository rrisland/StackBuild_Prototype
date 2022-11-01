using UnityEngine;
using UnityEngine.UI;

public class ImageAlphaTest : MonoBehaviour
{

    [SerializeField] private Image image;
    [SerializeField] private float threshold;

    private void Reset()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        image.alphaHitTestMinimumThreshold = threshold;
    }

}
