using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingController : MonoBehaviour
{
    public static bool begin;
    public float timer;
    private void Awake()
    {
        begin = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountDown());
    }
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(timer);
        begin = true;
    }
    // Update is called once per frame
}
