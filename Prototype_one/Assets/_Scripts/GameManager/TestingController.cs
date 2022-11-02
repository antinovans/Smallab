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
    private void Start()
    {
        SoundManager.instance.PlaySound("Background", true);
    }
    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            begin = true;
    }
    // Update is called once per frame
}
