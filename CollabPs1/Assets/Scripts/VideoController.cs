using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnEnd;
    }

    private void OnEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainScene");
    }
}
