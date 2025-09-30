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

        // Google Drive URL을 직접 다운로드 링크로 변환
        string directURL = ConvertGoogleDriveURL(testURL);
        pluginObject.Call("StreamMedia", directURL);
    }

    /// <summary>
    /// Google Drive 공유 링크를 직접 다운로드 링크로 변환
    /// </summary>
    /// <param name="googleDriveURL">Google Drive 공유 링크</param>
    /// <returns>직접 다운로드 링크</returns>
    private string ConvertGoogleDriveURL(string googleDriveURL)
    {
        if (string.IsNullOrEmpty(googleDriveURL))
            return googleDriveURL;

        // Google Drive URL 패턴 확인
        if (googleDriveURL.Contains("drive.google.com/file/d/"))
        {
            try
            {
                // URL에서 파일 ID 추출
                string fileId = ExtractFileIdFromGoogleDriveURL(googleDriveURL);
                if (!string.IsNullOrEmpty(fileId))
                {
                    // 직접 다운로드 링크로 변환
                    string directURL = $"https://drive.google.com/uc?export=download&id={fileId}";
                    Debug.Log($"Converted URL: {directURL}");
                    return directURL;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"URL 변환 실패: {e.Message}");
            }
        }

        Debug.LogWarning("Google Drive URL이 아니거나 변환할 수 없는 형식입니다.");
        return googleDriveURL; // 원본 URL 반환
    }

    /// <summary>
    /// Google Drive URL에서 파일 ID 추출
    /// </summary>
    /// <param name="url">Google Drive URL</param>
    /// <returns>파일 ID</returns>
    private string ExtractFileIdFromGoogleDriveURL(string url)
    {
        try
        {
            // "/file/d/" 이후와 "/view" 이전의 문자열 추출
            int startIndex = url.IndexOf("/file/d/") + "/file/d/".Length;
            int endIndex = url.IndexOf("/", startIndex);

            if (endIndex == -1)
            {
                // "/view"가 없는 경우 "?" 또는 끝까지
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
            Debug.LogError($"파일 ID 추출 실패: {e.Message}");
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
    /// Inspector에서 URL 변환 테스트용
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