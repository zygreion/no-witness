using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoStream : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.source = VideoSource.Url;
            string videoPath = Application.streamingAssetsPath + "/No_Witness_Game_Intro.mp4";
            videoPlayer.url = videoPath;

            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.prepareCompleted -= OnVideoPrepared;
    }
}