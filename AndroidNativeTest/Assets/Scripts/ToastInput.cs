using UnityEngine;
using UnityEngine.UI;

public class ToastInput : MonoBehaviour
{
    int showToastCount = 0;
    [SerializeField] Button showToastBtn;

    private void Awake()
    {
        showToastBtn.onClick.AddListener(OnShowToast);
    }

    void OnShowToast()
    {
        showToastCount++;
        AndroidNativeToast.ShowAndroidToastMessage("Show Toast Count: " + showToastCount);
    }
}
