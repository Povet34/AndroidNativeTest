using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MediaPlayer : MonoBehaviour
{
    public TextMeshProUGUI text;
    AndroidJavaClass plugin;
    AndroidJavaObject pluginObject;

    [SerializeField] string testURL = "https://drive.google.com/file/d/1J3yDKFGmwGmPsS5Rhk8P9ezFc7x9X2OY/view?usp=sharing";

    [SerializeField] Button playbtn;
    [SerializeField] Button pausebtn;
    [SerializeField] Button restartMusicBtn;

    private void Awake()
    {
        restartMusicBtn.onClick.AddListener(RestartMusic);
        pausebtn.onClick.AddListener(PauseMusic);
        playbtn.onClick.AddListener(PlayMusic);
    }

    private void Start()
    {
        plugin = new AndroidJavaClass("com.example.musicplayer.Musicplayer");
        pluginObject = new AndroidJavaObject("com.example.musicplayer.Musicplayer");

        // Google Drive URL�� ���� �ٿ�ε� ��ũ�� ��ȯ
        string directURL = ConvertGoogleDriveURL(testURL);
        pluginObject.Call("StreamMedia", directURL);
    }

    /// <summary>
    /// Google Drive ���� ��ũ�� ���� �ٿ�ε� ��ũ�� ��ȯ
    /// </summary>
    /// <param name="googleDriveURL">Google Drive ���� ��ũ</param>
    /// <returns>���� �ٿ�ε� ��ũ</returns>
    private string ConvertGoogleDriveURL(string googleDriveURL)
    {
        if (string.IsNullOrEmpty(googleDriveURL))
            return googleDriveURL;

        // Google Drive URL ���� Ȯ��
        if (googleDriveURL.Contains("drive.google.com/file/d/"))
        {
            try
            {
                // URL���� ���� ID ����
                string fileId = ExtractFileIdFromGoogleDriveURL(googleDriveURL);
                if (!string.IsNullOrEmpty(fileId))
                {
                    // ���� �ٿ�ε� ��ũ�� ��ȯ
                    string directURL = $"https://drive.google.com/uc?export=download&id={fileId}";
                    Debug.Log($"Converted URL: {directURL}");
                    return directURL;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"URL ��ȯ ����: {e.Message}");
            }
        }

        Debug.LogWarning("Google Drive URL�� �ƴϰų� ��ȯ�� �� ���� �����Դϴ�.");
        return googleDriveURL; // ���� URL ��ȯ
    }

    /// <summary>
    /// Google Drive URL���� ���� ID ����
    /// </summary>
    /// <param name="url">Google Drive URL</param>
    /// <returns>���� ID</returns>
    private string ExtractFileIdFromGoogleDriveURL(string url)
    {
        try
        {
            // "/file/d/" ���Ŀ� "/view" ������ ���ڿ� ����
            int startIndex = url.IndexOf("/file/d/") + "/file/d/".Length;
            int endIndex = url.IndexOf("/", startIndex);

            if (endIndex == -1)
            {
                // "/view"�� ���� ��� "?" �Ǵ� ������
                endIndex = url.IndexOf("?", startIndex);
                if (endIndex == -1)
                    endIndex = url.Length;
            }

            if (startIndex > 0 && endIndex > startIndex)
            {
                string fileId = url.Substring(startIndex, endIndex - startIndex);
                Debug.Log($"Extracted File ID: {fileId}");
                return fileId;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���� ID ���� ����: {e.Message}");
        }

        return null;
    }

    private void OnDestroy()
    {
        text.text = pluginObject.Call<string>("onDestroy");
    }

    public void PlayMusic()
    {
        string directURL = ConvertGoogleDriveURL(testURL);
        text.text = pluginObject.Call<string>("playAudio", directURL);
    }

    public void PauseMusic()
    {
        text.text = pluginObject.Call<string>("pauseAudio");
    }

    public void RestartMusic()
    {
        text.text = pluginObject.Call<string>("restart");
    }

    public void GetDuration()
    {
        text.text = pluginObject.Call<int>("getDuration").ToString();
    }

    public void GetCurrentPosition()
    {
        text.text = pluginObject.Call<int>("getCurrentPosition").ToString();
    }

    /// <summary>
    /// Inspector���� URL ��ȯ �׽�Ʈ��
    /// </summary>
    [ContextMenu("Test URL Conversion")]
    private void TestURLConversion()
    {
        string converted = ConvertGoogleDriveURL(testURL);
        Debug.Log($"Original: {testURL}");
        Debug.Log($"Converted: {converted}");

        if (text != null)
        {
            text.text = $"Converted URL: {converted}";
        }
    }
}