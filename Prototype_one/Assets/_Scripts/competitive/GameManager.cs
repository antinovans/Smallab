using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isStarted)
        {
            StartGame();
            isStarted = true;
        }

    }

    public void StartGame()
    {
        BoardManager.instance.InitializeBoard();
        //UI Start Count down
        StartCoroutine(GameStartCountDown(3.0f));
    }

    public void EndGame()
    {
        BoardManager.instance.DisableAllCubes();
        BoardManager.instance.ShowWinners();
        isStarted = false;
    }

    IEnumerator GameStartCountDown(float time)
    {
        float timer = time;
        while (timer >= 1)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("0");
            yield return null;
        }
        timer = 0;
        timerText.text = timer.ToString("0");
        BoardManager.instance.EnableAllCubes();
        StartCoroutine(GameEndCountDown(this.gameDuration));
    }
    IEnumerator GameEndCountDown(float time)
    {
        float timer = time;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("0");
            yield return null;
        }
        timer = 0;
        timerText.text = timer.ToString("0");
        EndGame();
    }

}
