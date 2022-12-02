using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    private float gameDuration;
    private bool isStarted = false;
    [Header("Count Down Text")]
    [SerializeField]
    private TextMeshProUGUI timerText;
    private VideoPlayer videoPlayer;
    [Header("Videos")]
    [SerializeField]
    VideoClip tutorial;
    [SerializeField]
    VideoClip countdown;

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
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlaySound("Background", true);
        videoPlayer = GetComponent<VideoPlayer>();
        /*videoPlayer.clip = tutorial;
        videoPlayer.Play();*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(PlayVideo(tutorial));
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isStarted)
        {
            StartGame();
            isStarted = true;
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
        videoPlayer.targetCameraAlpha = 0.0f;
    }

    public void StartGame()
    {
        //UI Start Count down
        StartCoroutine(GameStartCountDown());
    }

    public void EndGame()
    {
        BoardManager.instance.DisableAllCubes();
        BoardManager.instance.ShowWinners();
        SoundManager.instance.PlaySound("Win", false);
        isStarted = false;
    }

    IEnumerator GameStartCountDown()
    {
        videoPlayer.targetCameraAlpha = 1.0f;
        videoPlayer.clip = countdown;
        videoPlayer.Play();
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        videoPlayer.targetCameraAlpha = 0.0f;
        BoardManager.instance.InitializeBoard();
        BoardManager.instance.EnableAllCubes();
        StartCoroutine(GameEndCountDown(this.gameDuration));
    }
    IEnumerator GameEndCountDown(float time)
    {
        float timer = time;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            if (Mathf.Abs(timer - 60.0f) <= 0.1f)
                SoundManager.instance.PlaySound("60", false);
            if (Mathf.Abs(timer - 30.0f) <= 0.1f)
                SoundManager.instance.PlaySound("30", false);
            timerText.text = timer.ToString("0");
            yield return null;
        }
        timer = 0;
        timerText.text = timer.ToString("0");
        EndGame();
    }

}
