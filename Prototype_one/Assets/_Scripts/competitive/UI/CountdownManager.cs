using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public static CountdownManager instance;
    private TextMeshProUGUI timerText;

    // Update is called once per frame
    IEnumerator CountDown(float time)
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
    }
    IEnumerator CountDown(float time, bool control)
    {
        float timer = time;
        while(timer >= 0)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("0");
            yield return null;
        }
        timer = 0;
        timerText.text = timer.ToString("0");
        control = !control;
    }
}
