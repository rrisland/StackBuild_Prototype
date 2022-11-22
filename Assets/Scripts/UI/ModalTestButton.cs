using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ModalTestButton : MonoBehaviour
{

    [SerializeField] private Button button;
    [SerializeField] private ModalBase modal;

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (modal.IsOpen)
            {
                modal.HideAsync().Forget();
            }
            else
            {
                modal.ShowAsync().Forget();
            }
        });
    }
    
}
