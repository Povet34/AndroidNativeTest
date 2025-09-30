using UnityEngine;
using UnityEngine.UI; // UI.Text�� ����ϱ� ���� �ʿ��մϴ�.

public class BatteryManager : MonoBehaviour
{
    public Text batteryInfoText;
    private AndroidJavaObject batteryMonitor;

    void Start()
    {
        // �ùٸ� ��Ű����� Ŭ���������� ����
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
            Debug.Log($"BatteryState �÷������� ���������� �ʱ�ȭ�Ǿ����ϴ�: {pluginName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"BatteryState �÷����� �ʱ�ȭ ����: {e.Message}");
        }
    }

    private void UpdateBatteryInfo()
    {
        if (batteryMonitor == null || batteryInfoText == null) return;

        try
        {
            // 1. getBatteryLevel() �޼ҵ带 ȣ���Ͽ� ���͸� �ܷ��� �����ɴϴ�.
            //    �ڹ��� int�� C#�� int�� ȣȯ�Ǹ�, ��ȯ���� �����Ƿ� Call<int>�� ����մϴ�.
            int level = batteryMonitor.Call<int>("getBatteryLevel");

            // 2. isCharging() �޼ҵ带 ȣ���Ͽ� ���� ���¸� �����ɴϴ�.
            //    �ڹ��� boolean�� C#�� bool�� ȣȯ�Ǹ�, ��ȯ���� �����Ƿ� Call<bool>�� ����մϴ�.
            bool isCharging = batteryMonitor.Call<bool>("isCharging");

            string chargingStatus = isCharging ? "���� ��" : "���� �ȵ�";

            batteryInfoText.text = $"���͸� ����: {level}% ({chargingStatus})";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���͸� ���� ������Ʈ ����: {e.Message}");
            if (batteryInfoText != null)
            {
                batteryInfoText.text = "���͸� ������ ������ �� �����ϴ�";
            }
        }
    }
}