using UnityEngine;
using UnityEngine.SceneManagement;  // Import this to load the next scene
using UnityEngine.Video;            // Import VideoPlayer to control video

public class NextSence : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Reference to the VideoPlayer component
    public string nextSceneName;     // Name of the next scene

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the video end event
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    // This method will be called when the video ends
    void OnVideoEnd(VideoPlayer vp)
    {
        // Load the next scene when the video finishes
        SceneManager.LoadScene(nextSceneName);
    }
}
