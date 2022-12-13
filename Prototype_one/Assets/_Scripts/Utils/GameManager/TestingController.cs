using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TestingController : MonoBehaviour
{
    public static TestingController instance;

    private VideoPlayer videoPlayer;
    [Header("Videos")]
    [SerializeField]
    VideoClip tutorial;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(PlayVideo(tutorial));
    }
    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(!videoPlayer.isPlaying)
                StartCoroutine(PlayVideo(tutorial));
        }
    }
    IEnumerator PlayVideo(VideoClip clip)
    {
        videoPlayer.targetCameraAlpha = 1.0f;
        videoPlayer.clip = clip;
        videoPlayer.Play();
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        while (videoPlayer.targetCameraAlpha > 0)
        {
            videoPlayer.targetCameraAlpha -= Time.deltaTime;
            yield return null;
        }
        videoPlayer.targetCameraAlpha = 0.0f;
    }
    // Update is called once per frame
}
