using UnityEngine;
using UnityEngine.UI; // UI.Text를 사용하기 위해 필요합니다.

public class BatteryManager : MonoBehaviour
{
    public Text batteryInfoText;
    private AndroidJavaObject batteryMonitor;

    void Start()
    {
        // 올바른 패키지명과 클래스명으로 수정
        InitializePlugin("com.example.batterystate.BatteryState");
    }

    void Update()
    {
        UpdateBatteryInfo();
    }

    private void InitializePlugin(string pluginName)
    {
        if (Application.platform != RuntimePlatform.Android) return;

        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            batteryMonitor = new AndroidJavaObject(pluginName, currentActivity);
            Debug.Log($"BatteryState 플러그인이 성공적으로 초기화되었습니다: {pluginName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"BatteryState 플러그인 초기화 실패: {e.Message}");
        }
    }

    private void UpdateBatteryInfo()
    {
        if (batteryMonitor == null || batteryInfoText == null) return;

        try
        {
            // 1. getBatteryLevel() 메소드를 호출하여 배터리 잔량을 가져옵니다.
            //    자바의 int는 C#의 int와 호환되며, 반환값이 있으므로 Call<int>를 사용합니다.
            int level = batteryMonitor.Call<int>("getBatteryLevel");

            // 2. isCharging() 메소드를 호출하여 충전 상태를 가져옵니다.
            //    자바의 boolean은 C#의 bool과 호환되며, 반환값이 있으므로 Call<bool>을 사용합니다.
            bool isCharging = batteryMonitor.Call<bool>("isCharging");

            string chargingStatus = isCharging ? "충전 중" : "충전 안됨";

            batteryInfoText.text = $"배터리 상태: {level}% ({chargingStatus})";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"배터리 정보 업데이트 실패: {e.Message}");
            if (batteryInfoText != null)
            {
                batteryInfoText.text = "배터리 정보를 가져올 수 없습니다";
            }
        }
    }
}